namespace IntervYouQuestions.Api.Services
{
    public interface IUserService
    {
        Task<UserResponse> RegisterUserAsync(RegisterUserRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<UserProfileResponse> GetUserProfileAsync(string userId);
        Task<UserProfileResponse> UpdateUserProfileAsync(UpdateUserProfileRequest request);
        Task<UserStatsResponse> GetUserStatsAsync(string userId);
    }
}
