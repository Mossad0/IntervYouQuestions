
namespace IntervYouQuestions.Api.Services;

public interface ITopicService
{
    Task<IEnumerable<Topic>> GetAllAsync();
    Task<Topic?> GetAsync(int Id);
    Task<Topic> AddAsync(Topic request);
    Task<bool> UpdateAsync(int id, Topic request);
    Task<bool> DeleteAsync(int id);
}
