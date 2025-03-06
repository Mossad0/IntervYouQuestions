namespace IntervYouQuestions.Api.Services;

public interface IQuestionOptionService
{
    Task<IEnumerable<QuestionOption>> GetAllAsync();
    Task<QuestionOption?> GetAsync(int id);
    Task<QuestionOption> AddAsync(QuestionOption request);
    Task<bool> UpdateAsync(int id, QuestionOption request);
    Task<bool> DeleteAsync(int id);
}
