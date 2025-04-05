using IntervYouQuestions.Api.Contracts.Requests;
using IntervYouQuestions.Api.Contracts.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntervYouQuestions.Api.Services
{
    public interface IInterviewService
    {
        Task<IEnumerable<InterviewResponse>> GetAllInterviewsAsync();
        Task<IEnumerable<InterviewResponse>> GetUserInterviewsAsync(string userId);
        Task<InterviewResponse> GetInterviewByIdAsync(int id);
        Task<IEnumerable<QuestionResponse>> GetQuestionsForInterviewAsync(int interviewId);
        Task<InterviewResponse> CreateInterviewAsync(CreateInterviewRequest request);
        Task<InterviewResponse> UpdateInterviewAsync(int id, UpdateInterviewRequest request);
        Task DeleteInterviewAsync(int id);
        Task<InterviewResponse> StartInterviewAsync(StartInterviewRequest request);
        Task<InterviewResponse> StartInterviewForUserAsync(string userId, int numberOfQuestions, int timeLimitInMinutes = 30);
    }
}