using IntervYouQuestions.Api.Authentication;
using IntervYouQuestions.Api.Authentication.Dto;
using System.Security.Claims;

namespace IntervYouQuestions.Api.Services
{
    public interface IUserService
    {
        //Task<UserResponse> RegisterUserAsync(RegisterUserRequest request);
        //Task<LoginResponse> LoginAsync(LoginRequest request);
        //Task<UserProfileResponse> GetUserProfileAsync(string userId);
        //Task<UserProfileResponse> UpdateUserProfileAsync(UpdateUserProfileRequest request);
        Task<UserStatsResponse> GetUserStatsAsync(string userId);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
            
        Task<List<Interview>> GetUserInterviewsAsync(string userId);
        public JwtTokenResult GenerateJwtToken(AppUser user, IList<string> roles);

    }
}