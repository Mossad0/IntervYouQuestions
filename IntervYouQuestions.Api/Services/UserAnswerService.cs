using IntervYouQuestions.Api.Contracts.Requests;
using IntervYouQuestions.Api.Contracts.Responses;
using IntervYouQuestions.Api.Entities;
using IntervYouQuestions.Api.Exceptions;
using IntervYouQuestions.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntervYouQuestions.Api.Services
{
    public class UserAnswerService : IUserAnswerService
    {
        private readonly InterviewModuleContext _context;
        // Assuming you have these string constants defined somewhere, or use literals directly
        private const string QuestionTypeMCQ = "mcq";
        private const string QuestionTypeEssay = "essay";
        private const string QuestionTypeMultipleChoice = "multiplechoice"; // Or whatever string you use for this

        public UserAnswerService(InterviewModuleContext context)
        {
            _context = context;
        }

        public async Task SaveUserAnswerAsync(SubmitAnswerRequest request)
        {
            // Check if the question exists (Type check not relevant here)
            var question = await _context.Questions.FindAsync(request.QuestionId);
            if (question == null)
                throw new NotFoundException($"Question with ID {request.QuestionId} not found");

            // Check if the interview exists
            var interview = await _context.Interviews.FindAsync(request.InterviewId);
            if (interview == null)
                throw new NotFoundException($"Interview with ID {request.InterviewId} not found");

            // Check if the question belongs to the interview
            var interviewQuestion = await _context.InterviewQuestions
                .FirstOrDefaultAsync(iq => iq.InterviewId == request.InterviewId && iq.QuestionId == request.QuestionId);
            if (interviewQuestion == null)
                throw new BadRequestException("This question does not belong to the specified interview");

            // Check if a previous answer exists and update or create new
            var existingAnswer = await _context.UserAnswers
                .FirstOrDefaultAsync(ua => ua.InterviewId == request.InterviewId &&
                                    ua.QuestionId == request.QuestionId &&
                                    ua.UserId == request.UserId); // Assuming UserId distinguishes users per interview

            if (existingAnswer != null)
            {
                // Update existing answer
                existingAnswer.AnswerText = request.AnswerText;
                // Ensure SelectedOptionIds is never null before Join
                existingAnswer.SelectedOptions = string.Join(",", request.SelectedOptionIds ?? Enumerable.Empty<int>());
                existingAnswer.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new answer
                var userAnswer = new UserAnswer
                {
                    UserId = request.UserId, // Make sure UserId is being passed correctly
                    InterviewId = request.InterviewId,
                    QuestionId = request.QuestionId,
                    AnswerText = request.AnswerText,
                    // Ensure SelectedOptionIds is never null before Join
                    SelectedOptions = string.Join(",", request.SelectedOptionIds ?? Enumerable.Empty<int>()),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.UserAnswers.Add(userAnswer);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<InterviewResultResponse> CalculateInterviewScoreAsync(int interviewId)
        {
            var interview = await _context.Interviews
                .Include(i => i.InterviewQuestions)
                    .ThenInclude(iq => iq.Question)
                        .ThenInclude(q => q.QuestionOptions) // Needed for grading MCQ/MultipleChoice
                .FirstOrDefaultAsync(i => i.Id == interviewId);

            if (interview == null)
                throw new NotFoundException($"Interview with ID {interviewId} not found");

            // Get all user answers for this specific interview instance.
            // If multiple users can take the same logical interview, you might need a unique InterviewAttemptId or filter by UserId.
            // Let's assume interviewId represents a single user's attempt for now.
            var userAnswers = await _context.UserAnswers
                .Where(ua => ua.InterviewId == interviewId)
                .ToListAsync();

            int totalQuestions = interview.InterviewQuestions.Count;
            // Count distinct questions answered for this interview attempt
            int answeredQuestions = userAnswers.Select(ua => ua.QuestionId).Distinct().Count();
            int correctAnswers = 0;

            var detailedResults = new List<QuestionResultResponse>();

            foreach (var interviewQuestion in interview.InterviewQuestions)
            {
                var question = interviewQuestion.Question;
                // Find the latest answer for this question in this interview attempt
                var userAnswer = userAnswers.LastOrDefault(ua => ua.QuestionId == question.QuestionId);
                bool isCorrect = false;
                string feedback = "Question not answered";
                string userAnswerText = null;
                List<int> userAnswerOptionIds = new List<int>();

                if (userAnswer != null)
                {
                    userAnswerText = userAnswer.AnswerText; // Store user's text answer
                    // Safely parse selected options
                    if (!string.IsNullOrEmpty(userAnswer.SelectedOptions))
                    {
                        try
                        {
                            userAnswerOptionIds = userAnswer.SelectedOptions
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(int.Parse)
                                .ToList();
                        }
                        catch (FormatException ex)
                        {
                            // Handle error: Log it, set feedback?
                            Console.WriteLine($"Error parsing selected options for UserAnswer ID : {ex.Message}");
                            feedback = "Error processing selected options.";
                            // Decide if this counts as incorrect or needs review
                            isCorrect = false;
                        }
                    }


                    // *** SWITCH ON STRING TYPE (Normalized) ***
                    // Normalize the type string for reliable comparison (lowercase, trimmed)
                    string questionTypeNormalized = question.Type?.ToLowerInvariant().Trim() ?? string.Empty;

                    switch (questionTypeNormalized)
                    {
                        // Case for Multiple Choice (assuming type string is "multiplechoice")
                        // Adjust the string literal if you use a different value (e.g., "multichoice")
                        case QuestionTypeMultipleChoice:
                            var correctOptionIdsMC = question.QuestionOptions
                                .Where(o => o.IsCorrect)
                                .Select(o => o.OptionId)
                                .ToHashSet(); // Use HashSet for efficient comparison

                            var selectedOptionIdsMC = userAnswerOptionIds.ToHashSet();

                            // Check if the set of selected IDs exactly matches the set of correct IDs
                            isCorrect = correctOptionIdsMC.SetEquals(selectedOptionIdsMC);

                            feedback = isCorrect
                                ? "Correct answer"
                                : "Incorrect answer."; // More detail could be added later
                            break;

                        // Case for Essay/Text answers
                        case QuestionTypeEssay:
                            isCorrect = false; // Cannot auto-grade
                            feedback = "Text answer requires manual review";
                            break;

                        // Case for Single Choice MCQ
                        case QuestionTypeMCQ:
                            if (userAnswerOptionIds.Count == 1) // Ensure only one option was selected
                            {
                                int selectedId = userAnswerOptionIds.First();
                                // Find the option selected by the user
                                var selectedOption = question.QuestionOptions.FirstOrDefault(o => o.OptionId == selectedId);
                                // Check if that option exists and is marked as correct
                                isCorrect = selectedOption?.IsCorrect ?? false;

                                feedback = isCorrect
                                    ? "Correct answer"
                                    : "Incorrect answer.";
                            }
                            else if (userAnswerOptionIds.Count == 0 && !string.IsNullOrEmpty(userAnswer.AnswerText))
                            {
                                // Edge case: Maybe it was stored as text? Unlikely for MCQ.
                                isCorrect = false;
                                feedback = "No option selected.";
                            }
                            else if (userAnswerOptionIds.Count > 1)
                            {
                                isCorrect = false;
                                feedback = "Multiple options selected for a single-choice question.";
                            }
                            else // No options selected, no text answer
                            {
                                feedback = "No answer provided."; // Override default "not answered"
                            }
                            break;

                        // Default case for unknown/unhandled types
                        default:
                            feedback = $"Unsupported question type '{question.Type}'. Cannot determine score.";
                            isCorrect = false;
                            break;
                    }

                    if (isCorrect)
                        correctAnswers++;
                } // end if (userAnswer != null)


                // *** Ensure QuestionResultResponse constructor matches its definition ***
                // Assuming it takes: id, text, type, userAnswerText, userAnswerOptionIds, isCorrect, feedback
                detailedResults.Add(new QuestionResultResponse(
                   question.QuestionId,
                    question.Text,
                    isCorrect,
                    feedback
               ));
            }

            double scorePercentage = totalQuestions > 0
                ? (double)correctAnswers / totalQuestions * 100
                : 0;

            // Update Interview Status if not already completed
            if (interview.Status != InterviewStatus.Completed)
            {
                interview.Status = InterviewStatus.Completed;
                interview.EndTime = DateTime.UtcNow;
                // You might want to store the score on the Interview entity as well
                // interview.CalculatedScore = scorePercentage;
                await _context.SaveChangesAsync();
            }

            // Ensure InterviewResultResponse constructor matches its definition
            return new InterviewResultResponse(
                interview.Id,
                interview.Title,
                interview.ExperienceLevel.ToString(), // Ensure ExperienceLevel enum exists and is handled
                totalQuestions,
                answeredQuestions,
                correctAnswers,
                scorePercentage,
                detailedResults
            );
        }
        // Ensure InterviewStatus enum exists
        // public enum InterviewStatus { Pending, InProgress, Completed, Cancelled }

        // Ensure QuestionResultResponse definition matches the call above
        // Example:
        // public record QuestionResultResponse(
        //     int QuestionId,
        //     string QuestionText,
        //     string QuestionType,
        //     string UserAnswerText,
        //     List<int> UserSelectedOptionIds,
        //     bool IsCorrect,
        //     string Feedback
        // );

        // Ensure InterviewResultResponse definition matches the call above
        // Example:
        // public record InterviewResultResponse(
        //    int InterviewId,
        //    string Title,
        //    string ExperienceLevel,
        //    int TotalQuestions,
        //    int AnsweredQuestions,
        //    int CorrectAnswers,
        //    double ScorePercentage,
        //    List<QuestionResultResponse> DetailedResults
        // );
    }
}
