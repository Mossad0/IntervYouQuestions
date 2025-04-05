namespace IntervYouQuestions.Api.Services
{
    public interface IInterviewExpirationService
    {
        Task CheckAndUpdateExpiredInterviewsAsync();
        Task<bool> IsInterviewExpiredAsync(int interviewId);
        Task<DateTime> GetInterviewExpirationDateAsync(int interviewId);
    }
} 