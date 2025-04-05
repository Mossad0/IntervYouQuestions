namespace IntervYouQuestions.Api.Services
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> UpdateUserAsync(User user);
        Task<UserProfile> GetUserProfileAsync(string userId);
        Task<UserProfile> CreateUserProfileAsync(UserProfile profile);
        Task<bool> UpdateUserProfileAsync(UserProfile profile);
        Task<List<Interview>> GetUserInterviewsAsync(string userId);
    }
}
