using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IntervYouQuestions.Api.Entities;
using IntervYouQuestions.Api.Contracts.Requests;
using IntervYouQuestions.Api.Contracts.Responses;

namespace IntervYouQuestions.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(
            IUserRepository userRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<UserResponse> RegisterUserAsync(RegisterUserRequest request)
        {
            // Check if user with email already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return null; // User already exists
            }
            var hashedPassword = HashPassword(request.Password);
            Console.WriteLine($"Hashed Password: {hashedPassword}");


            // Create new user
            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Username = request.Username,
                Email = request.Email,
                Password = hashedPassword, // هنا المفروض يكون محفوظ
                ExperienceLevel = request.ExperienceLevel,
                Role = request.Role,
                RegisteredDate = DateTime.UtcNow,
                IsActive = true
            };

            Console.WriteLine($"User Object Before Save: {user.UserId}, {user.Email}, {user.Password}");


            var createdUser = await _userRepository.CreateUserAsync(user);

            Console.WriteLine($"Created User Password: {createdUser.Password}");


            // Create a default profile for the user
            var profile = new UserProfile
            {
                UserId = user.UserId,
                FullName = user.Username, // Default to username
                TotalInterviewsTaken = 0,
                TotalInterviewsCompleted = 0,
                AverageScore = 0
            };

            await _userRepository.CreateUserProfileAsync(profile);

            // Map to response
            return new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                ExperienceLevel = user.ExperienceLevel,
                Role = user.Role,
                RegisteredDate = user.RegisteredDate
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Get user by email
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return null; // User not found
            }

            // Verify password
            if (string.IsNullOrEmpty(user.Password) || user.Password != HashPassword(request.Password))
            {
                return null; // أو يمكنك إرجاع Unauthorized
            }


            // Generate JWT token
            var token = GenerateJwtToken(user);

            // Return login response
            return new LoginResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                ExperienceLevel = user.ExperienceLevel,
                Role = user.Role,
                Token = token
            };
        }

        public async Task<UserProfileResponse> GetUserProfileAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var profile = await _userRepository.GetUserProfileAsync(userId);
            if (profile == null)
            {
                // Create a default profile if none exists
                profile = new UserProfile
                {
                    UserId = userId,
                    FullName = user.Username
                };
                await _userRepository.CreateUserProfileAsync(profile);
            }

            // Map to response
            return new UserProfileResponse
            {
                ProfileId = profile.ProfileId,
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = profile.FullName,
                PhoneNumber = profile.PhoneNumber,
                LinkedInProfile = profile.LinkedInProfile,
                GitHubProfile = profile.GitHubProfile,
                Biography = profile.Biography,
                ExperienceLevel = user.ExperienceLevel,
                Role = user.Role,
                TotalInterviewsTaken = profile.TotalInterviewsTaken,
                TotalInterviewsCompleted = profile.TotalInterviewsCompleted,
                AverageScore = profile.AverageScore
            };
        }

        public async Task<UserProfileResponse> UpdateUserProfileAsync(UpdateUserProfileRequest request)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                return null;
            }

            var profile = await _userRepository.GetUserProfileAsync(request.UserId);
            if (profile == null)
            {
                // Create a new profile
                profile = new UserProfile
                {
                    UserId = request.UserId,
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    LinkedInProfile = request.LinkedInProfile,
                    GitHubProfile = request.GitHubProfile,
                    Biography = request.Biography
                };
                await _userRepository.CreateUserProfileAsync(profile);
            }
            else
            {
                // Update existing profile
                profile.FullName = request.FullName;
                profile.PhoneNumber = request.PhoneNumber;
                profile.LinkedInProfile = request.LinkedInProfile;
                profile.GitHubProfile = request.GitHubProfile;
                profile.Biography = request.Biography;
                await _userRepository.UpdateUserProfileAsync(profile);
            }

            // Map to response
            return await GetUserProfileAsync(request.UserId);
        }

        public async Task<UserStatsResponse> GetUserStatsAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var profile = await _userRepository.GetUserProfileAsync(userId);
            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    TotalInterviewsTaken = 0,
                    TotalInterviewsCompleted = 0,
                    AverageScore = 0
                };
            }

            // Get user interviews
            var interviews = await _userRepository.GetUserInterviewsAsync(userId);

            // Calculate statistics
            var completedInterviews = interviews.Where(i => i.Status == InterviewStatus.Completed).ToList();
            var avgScore = completedInterviews.Any()
                ? completedInterviews.Average(i => CalculateInterviewScore(i))
                : 0;

            // Map recent interviews
            var recentInterviews = interviews
                .OrderByDescending(i => i.StartTime)
                .Take(5)
                .Select(i => new InterviewSummary
                {
                    InterviewId = i.Id.ToString(),
                    Role = i.Role,
                    ExperienceLevel = i.ExperienceLevel.ToString(),
                    Date = i.StartTime.ToString("yyyy-MM-dd"),
                    Score = CalculateInterviewScore(i),
                    QuestionsAnswered = i.InterviewQuestions.Count,
                    TotalQuestions = i.InterviewQuestions.Count
                })
                .ToList();

            // Calculate topic performance
            var topicPerformance = new Dictionary<string, double>();
            foreach (var interview in completedInterviews)
            {
                foreach (var question in interview.InterviewQuestions.Select(iq => iq.Question))
                {
                    if (question?.Topic == null) continue;

                    var topicName = question.Topic.Name;
                    if (!topicPerformance.ContainsKey(topicName))
                    {
                        topicPerformance[topicName] = 0;
                    }

                    var correctAnswers = interview.InterviewQuestions.Count(iq => iq.QuestionId == question.QuestionId && iq.Question.QuestionOptions.Any(qo => qo.IsCorrect));
                    topicPerformance[topicName] += correctAnswers;
                }
            }

            // Normalize topic performance to percentages
            foreach (var topic in topicPerformance.Keys.ToList())
            {
                var totalQuestionsForTopic = completedInterviews
                    .SelectMany(i => i.InterviewQuestions)
                    .Count(iq => iq.Question?.Topic?.Name == topic);

                if (totalQuestionsForTopic > 0)
                {
                    topicPerformance[topic] = (topicPerformance[topic] / totalQuestionsForTopic) * 100;
                }
            }

            // Return stats
            return new UserStatsResponse
            {
                UserId = userId,
                TotalInterviews = interviews.Count,
                CompletedInterviews = completedInterviews.Count,
                AverageScore = avgScore,
                RecentInterviews = recentInterviews,
                TopicPerformance = topicPerformance
            };
        }

        private string HashPassword(string password)
        {
            // In a real application, use a proper password hashing algorithm
            // This is just a simple example for demonstration purposes
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Role),
                new Claim("experience", user.ExperienceLevel),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private double CalculateInterviewScore(Interview interview)
        {
            if (interview.Status != InterviewStatus.Completed || !interview.InterviewQuestions.Any())
            {
                return 0;
            }

            var correctAnswers = interview.InterviewQuestions.Count(iq => iq.Question.QuestionOptions.Any(qo => qo.IsCorrect));
            return (double)correctAnswers / interview.InterviewQuestions.Count * 100;
        }
    }
}




