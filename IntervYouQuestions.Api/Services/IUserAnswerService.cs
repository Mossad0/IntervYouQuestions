namespace IntervYouQuestions.Api.Services
{
    public interface IUserAnswerService
    {
        Task SaveUserAnswerAsync(SubmitAnswerRequest request);
        Task<InterviewResultResponse> CalculateInterviewScoreAsync(int interviewId);
    }
}