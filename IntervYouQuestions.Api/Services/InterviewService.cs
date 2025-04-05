using IntervYouQuestions.Api.Contracts.Requests;
using IntervYouQuestions.Api.Contracts.Responses;
using IntervYouQuestions.Api.Entities;
using IntervYouQuestions.Api.Persistence;
using IntervYouQuestions.Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntervYouQuestions.Api.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly InterviewModuleContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public InterviewService(
            InterviewModuleContext context,
            IMapper mapper,
            IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IEnumerable<InterviewResponse>> GetAllInterviewsAsync()
        {
            var interviews = await _context.Interviews
                .Include(i => i.InterviewQuestions)
                .ThenInclude(iq => iq.Question)
                .ToListAsync();

            return _mapper.Map<IEnumerable<InterviewResponse>>(interviews);
        }

        public async Task<IEnumerable<InterviewResponse>> GetUserInterviewsAsync(string userId)
        {
            var interviews = await _context.Interviews
                .Where(i => i.UserId == userId)
                .Include(i => i.InterviewQuestions)
                .ThenInclude(iq => iq.Question)
                .ToListAsync();

            return _mapper.Map<IEnumerable<InterviewResponse>>(interviews);
        }
        public async Task<IEnumerable<QuestionResponse>> GetQuestionsForInterviewAsync(int interviewId)
        {
            var interviewExists = await _context.Interviews.AnyAsync(i => i.Id == interviewId);
            if (!interviewExists)
            {
                throw new NotFoundException($"Interview with ID {interviewId} not found.");
            }

            var questions = await _context.InterviewQuestions
                .Where(iq => iq.InterviewId == interviewId)
                .Include(iq => iq.Question)
                    .ThenInclude(q => q.QuestionOptions) // Include options for mapping
                                                         // .Include(iq => iq.Question)            // Don't include ModelAnswers here
                                                         //    .ThenInclude(q => q.ModelAnswers)   // Avoid sending answers to the user taking test
                .OrderBy(iq => iq.OrderIndex)
                .Select(iq => iq.Question) // Select the Question entity
                                           // *** THIS IS THE UPDATED MAPPING PART ***
                .Select(q => new QuestionResponse( // Use the record constructor
                    q.QuestionId,                  // QuestionId (int)
                    q.Type.ToString(),             // Type (string - converted from enum)
                    q.Text,                        // Text (string)
                    q.Difficulty,                  // Difficulty (string - assumes Question entity has this)
                    q.TopicId,                     // TopicId (int - assumes Question entity has this)
                    q.QuestionOptions.Select(o => new QuestionOptionResponse( // Use constructor ( )
                 o.OptionId,  // First argument for OptionId
                 o.Text       // Second argument for Text
            )).ToList(),                    // QuestionOptions (IEnumerable<QuestionOptionResponse>?) - Use ToList()
                    null                           // ModelAnswers - SET TO NULL deliberately
                                                   // You do NOT want to send the model answers to the client
                                                   // when they are supposed to be answering the questions.
                                                   // Pass null or Enumerable.Empty<ModelAnswerResponse>()
                ))
                .ToListAsync();

            return questions;
        }

        public async Task<InterviewResponse> GetInterviewByIdAsync(int id)
        {
            var interview = await _context.Interviews
                .Include(i => i.InterviewQuestions)
                .ThenInclude(iq => iq.Question)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (interview == null)
                throw new NotFoundException($"Interview with ID {id} not found");

            return _mapper.Map<InterviewResponse>(interview);
        }

        public async Task<InterviewResponse> CreateInterviewAsync(CreateInterviewRequest request)
        {
            var interview = _mapper.Map<Interview>(request);
            interview.CreatedDate = DateTime.UtcNow;
            interview.Status = InterviewStatus.Draft;
            interview.StartTime = DateTime.UtcNow;

            _context.Interviews.Add(interview);
            await _context.SaveChangesAsync();

            // Add interview questions
            int orderIndex = 0;
            foreach (var questionId in request.QuestionIds)
            {
                _context.InterviewQuestions.Add(new InterviewQuestion
                {
                    InterviewId = interview.Id,
                    QuestionId = questionId,
                    OrderIndex = orderIndex++
                });
            }

            await _context.SaveChangesAsync();

            return await GetInterviewByIdAsync(interview.Id);
        }

        public async Task<InterviewResponse> UpdateInterviewAsync(int id, UpdateInterviewRequest request)
        {
            var interview = await _context.Interviews
                .Include(i => i.InterviewQuestions)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (interview == null)
                throw new NotFoundException($"Interview with ID {id} not found");

            // Update interview properties
            interview.Title = request.Title;
            interview.Description = request.Description;
            interview.ExpirationDate = request.ExpirationDate;
            interview.ExperienceLevel = request.ExperienceLevel;
            interview.Status = request.Status;

            // Update interview questions
            _context.InterviewQuestions.RemoveRange(interview.InterviewQuestions);
            await _context.SaveChangesAsync();

            // Add updated questions
            int orderIndex = 0;
            foreach (var questionId in request.QuestionIds)
            {
                _context.InterviewQuestions.Add(new InterviewQuestion
                {
                    InterviewId = interview.Id,
                    QuestionId = questionId,
                    OrderIndex = orderIndex++
                });
            }

            await _context.SaveChangesAsync();

            return await GetInterviewByIdAsync(interview.Id);
        }

        public async Task DeleteInterviewAsync(int id)
        {
            var interview = await _context.Interviews.FindAsync(id);
            if (interview == null)
                throw new NotFoundException($"Interview with ID {id} not found");

            _context.Interviews.Remove(interview);
            await _context.SaveChangesAsync();
        }

        // Method for starting an interview with dynamic difficulty distribution and category weighting
        public async Task<InterviewResponse> StartInterviewAsync(StartInterviewRequest request)
        {
            // Define difficulty distribution based on experience level
            var difficultyDistribution = GetDifficultyDistributionByLevel(request.ExperienceLevel, request.NumberOfQuestions);

            // Define category weights
            var categoryWeights = new Dictionary<string, int>
            {
                { "Database", 25 },
                { "C#", 30 },
                { "C# OOP", 20 },
                { ".NET Web API", 15 },
                { ".NET Web MVC", 10 }
            };

            // Calculate number of questions per category
            var questionCountByCategory = CalculateQuestionCountByCategory(categoryWeights, request.NumberOfQuestions);

            // Get all questions with their categories and topics
            var allQuestions = await _context.Questions
                .Include(q => q.Topic)
                .ThenInclude(t => t.Category)
                .Include(q => q.QuestionOptions)
                .Include(q => q.ModelAnswers)
                .ToListAsync();

            // Group questions by category
            var questionsByCategory = allQuestions
                .GroupBy(q => q.Topic.Category.Name)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Group questions by difficulty within each category
            var selectedQuestions = new List<Question>();

            // For each category, select questions according to difficulty distribution and category weight
            foreach (var category in questionCountByCategory.Keys)
            {
                int categoryCount = questionCountByCategory[category];

                // Skip if we don't have questions for this category
                if (!questionsByCategory.ContainsKey(category) || !questionsByCategory[category].Any())
                    continue;

                // Group questions in this category by difficulty
                var categoryQuestionsByDifficulty = questionsByCategory[category]
                    .GroupBy(q => q.Difficulty)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Apply difficulty distribution within this category
                var categoryDifficultyDistribution = new Dictionary<string, int>();
                foreach (var difficulty in difficultyDistribution.Keys)
                {
                    // Calculate proportional number of questions for this difficulty in this category
                    double proportion = (double)difficultyDistribution[difficulty] / request.NumberOfQuestions;
                    categoryDifficultyDistribution[difficulty] = (int)Math.Round(categoryCount * proportion);
                }

                // Ensure we're selecting exactly categoryCount questions
                int totalAllocated = categoryDifficultyDistribution.Values.Sum();
                if (totalAllocated < categoryCount)
                {
                    // Add remaining to medium difficulty
                    categoryDifficultyDistribution["intermediate"] += (categoryCount - totalAllocated);
                }
                else if (totalAllocated > categoryCount)
                {
                    // Remove excess from easy first, then medium, then hard
                    int excess = totalAllocated - categoryCount;
                    if (categoryDifficultyDistribution["easy"] >= excess)
                    {
                        categoryDifficultyDistribution["easy"] -= excess;
                    }
                    else
                    {
                        excess -= categoryDifficultyDistribution["easy"];
                        categoryDifficultyDistribution["easy"] = 0;

                        if (categoryDifficultyDistribution["intermediate"] >= excess)
                        {
                            categoryDifficultyDistribution["intermediate"] -= excess;
                        }
                        else
                        {
                            excess -= categoryDifficultyDistribution["intermediate"];
                            categoryDifficultyDistribution["intermediate"] = 0;
                            categoryDifficultyDistribution["hard"] -= excess;
                        }
                    }
                }

                // Select questions for this category by difficulty
                foreach (var difficulty in categoryDifficultyDistribution.Keys)
                {
                    int count = categoryDifficultyDistribution[difficulty];

                    if (!categoryQuestionsByDifficulty.ContainsKey(difficulty) || !categoryQuestionsByDifficulty[difficulty].Any())
                        continue;

                    // Get random questions for this difficulty and category
                    var availableQuestions = categoryQuestionsByDifficulty[difficulty]
                        .OrderBy(q => Guid.NewGuid())
                        .Take(count)
                        .ToList();

                    selectedQuestions.AddRange(availableQuestions);
                }
            }

            // If we don't have enough questions from our preferred categories, fill with random ones
            if (selectedQuestions.Count < request.NumberOfQuestions)
            {
                var remainingCount = request.NumberOfQuestions - selectedQuestions.Count;
                var alreadySelectedIds = selectedQuestions.Select(q => q.QuestionId).ToHashSet();

                var additionalQuestions = allQuestions
                    .Where(q => !alreadySelectedIds.Contains(q.QuestionId))
                    .OrderBy(q => Guid.NewGuid())
                    .Take(remainingCount)
                    .ToList();

                selectedQuestions.AddRange(additionalQuestions);
            }
            // If we have too many questions, remove extras
            else if (selectedQuestions.Count > request.NumberOfQuestions)
            {
                selectedQuestions = selectedQuestions
                    .OrderBy(q => Guid.NewGuid())
                    .Take(request.NumberOfQuestions)
                    .ToList();
            }

            // Create new interview
            var interview = new Interview
            {
                Title = $"{request.Role} Interview for {request.ExperienceLevel} Level",
                Description = $"Auto-generated interview with {selectedQuestions.Count} questions",
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddMinutes(request.TimeLimitInMinutes),
                Status = InterviewStatus.Active,
                ExperienceLevel = request.ExperienceLevel,
                StartTime = DateTime.UtcNow,
                Role = request.Role.ToString(),
                UserId = request.UserId
            };

            _context.Interviews.Add(interview);
            await _context.SaveChangesAsync();

            // Add interview questions
            int orderIndex = 0;
            foreach (var question in selectedQuestions)
            {
                _context.InterviewQuestions.Add(new InterviewQuestion
                {
                    InterviewId = interview.Id,
                    QuestionId = question.QuestionId,
                    OrderIndex = orderIndex++
                });
            }

            await _context.SaveChangesAsync();

            // Prepare the response
            var questionResponses = selectedQuestions.Select(q => MapToQuestionResponse(q)).ToList();
            var response = new InterviewResponse
            {
                InterviewId = interview.Id.ToString(),
                ExperienceLevel = interview.ExperienceLevel,
                Role = request.Role,
                TimeLimitInMinutes = request.TimeLimitInMinutes,
                QuestionIds = selectedQuestions.Select(q => q.QuestionId).ToList(),
                CurrentQuestion = questionResponses.FirstOrDefault()
            };

            return response;
        }

        // Helper method to calculate number of questions per category based on weights
        private Dictionary<string, int> CalculateQuestionCountByCategory(Dictionary<string, int> categoryWeights, int totalQuestions)
        {
            var result = new Dictionary<string, int>();
            int totalWeight = categoryWeights.Values.Sum();

            foreach (var category in categoryWeights.Keys)
            {
                // Calculate proportional questions for this category
                double proportion = (double)categoryWeights[category] / totalWeight;
                result[category] = (int)Math.Round(totalQuestions * proportion);
            }

            // Ensure the sum matches totalQuestions
            int allocatedQuestions = result.Values.Sum();
            if (allocatedQuestions < totalQuestions)
            {
                // Add remaining questions to the highest weight category
                var highestWeightCategory = categoryWeights.OrderByDescending(kv => kv.Value).First().Key;
                result[highestWeightCategory] += (totalQuestions - allocatedQuestions);
            }
            else if (allocatedQuestions > totalQuestions)
            {
                // Remove excess questions from the lowest weight category
                var lowestWeightCategory = categoryWeights.OrderBy(kv => kv.Value).First().Key;

                if (result[lowestWeightCategory] >= (allocatedQuestions - totalQuestions))
                {
                    result[lowestWeightCategory] -= (allocatedQuestions - totalQuestions);
                }
                else
                {
                    // If lowest category doesn't have enough, go to next lowest
                    int remaining = allocatedQuestions - totalQuestions - result[lowestWeightCategory];
                    result[lowestWeightCategory] = 0;

                    foreach (var category in categoryWeights.OrderBy(kv => kv.Value).Skip(1))
                    {
                        if (result[category.Key] >= remaining)
                        {
                            result[category.Key] -= remaining;
                            break;
                        }
                        else
                        {
                            remaining -= result[category.Key];
                            result[category.Key] = 0;
                        }
                    }
                }
            }

            return result;
        }

        // New method to start an interview based on user's profile
        public async Task<InterviewResponse> StartInterviewForUserAsync(string userId, int numberOfQuestions, int timeLimitInMinutes = 30)
        {
            // Get user profile
            var userProfile = await _userService.GetUserProfileAsync(userId);
            if (userProfile == null)
                throw new NotFoundException($"User with ID {userId} not found");

            // Create a StartInterviewRequest based on user's profile
            var request = new StartInterviewRequest
            {
                ExperienceLevel = Enum.Parse<ExperienceLevel>(userProfile.ExperienceLevel),
                Role = Enum.Parse<RoleType>(userProfile.Role),
                NumberOfQuestions = numberOfQuestions,
                TimeLimitInMinutes = timeLimitInMinutes,
                UserId = userId
            };

            // Use the existing StartInterviewAsync method
            return await StartInterviewAsync(request);
        }

        // Helper method to get difficulty distribution based on experience level
        private Dictionary<string, int> GetDifficultyDistributionByLevel(ExperienceLevel level, int totalQuestions)
        {
            var distribution = new Dictionary<string, int>();

            switch (level)
            {
                case ExperienceLevel.Junior:
                    // For junior: 60% Easy, 30% Medium, 10% Hard
                    distribution["easy"] = (int)(totalQuestions * 0.6);
                    distribution["intermediate"] = (int)(totalQuestions * 0.3);
                    distribution["hard"] = totalQuestions - distribution["easy"] - distribution["intermediate"];
                    break;

                case ExperienceLevel.MidLevel:
                    // For intermediate: 30% Easy, 50% Medium, 20% Hard
                    distribution["easy"] = (int)(totalQuestions * 0.3);
                    distribution["intermediate"] = (int)(totalQuestions * 0.5);
                    distribution["hard"] = totalQuestions - distribution["easy"] - distribution["intermediate"];
                    break;

                case ExperienceLevel.Senior:
                    // For senior: 10% Easy, 40% Medium, 50% Hard
                    distribution["easy"] = (int)(totalQuestions * 0.1);
                    distribution["intermediate"] = (int)(totalQuestions * 0.4);
                    distribution["hard"] = totalQuestions - distribution["easy"] - distribution["intermediate"];
                    break;

                case ExperienceLevel.Lead:
                    // For lead: 0% Easy, 30% Medium, 70% Hard
                    distribution["easy"] = 0;
                    distribution["intermediate"] = (int)(totalQuestions * 0.3);
                    distribution["hard"] = totalQuestions - distribution["easy"] - distribution["intermediate"];
                    break;

                default:
                    // Default even distribution
                    distribution["easy"] = totalQuestions / 3;
                    distribution["intermediate"] = totalQuestions / 3;
                    distribution["hard"] = totalQuestions - distribution["easy"] - distribution["intermediate"];
                    break;
            }

            // Ensure no negative values
            foreach (var key in distribution.Keys.ToList())
            {
                if (distribution[key] < 0)
                    distribution[key] = 0;
            }

            return distribution;
        }

        // Helper method to map Question entity to QuestionResponse
        private QuestionResponse MapToQuestionResponse(Question question)
        {
            if (!_context.Entry(question).Collection(q => q.QuestionOptions).IsLoaded)
            {
                _context.Entry(question).Collection(q => q.QuestionOptions).Load();
            }

            var options = question.QuestionOptions.Select(o => new QuestionOptionResponse(
                o.OptionId,  // Use the option's own ID, not QuestionId
                o.Text
                //o.IsCorrect
            )).ToList();

            if (!_context.Entry(question).Collection(q => q.ModelAnswers).IsLoaded)
            {
                _context.Entry(question).Collection(q => q.ModelAnswers).Load();
            }

            var modelAnswers = question.ModelAnswers.Select(m => new ModelAnswerResponse(
                m.ModelAnswerId,
                m.Text,
                m.KeyPoints
            )).ToList();

            return new QuestionResponse(
                question.QuestionId,
                question.Type.ToString(),
                question.Text,
                question.Difficulty,
                question.TopicId,
                options,
                modelAnswers
            );
        }
    }
}

