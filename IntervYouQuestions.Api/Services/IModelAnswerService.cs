namespace IntervYouQuestions.Api.Services;

public interface IModelAnswerService
{
    Task<IEnumerable<ModelAnswer>> GetAllAsync();
    Task<ModelAnswer?> GetAsync(int id);
    //Task<IEnumerable<ModelAnswer>> GetByQuestionIdAsync(int questionId);
    Task<ModelAnswer> AddAsync(ModelAnswer request);
    Task<bool> UpdateAsync(int id, ModelAnswer request);
    Task<bool> DeleteAsync(int id);
}
