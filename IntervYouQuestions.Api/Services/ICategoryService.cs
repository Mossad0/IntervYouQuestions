
namespace IntervYouQuestions.Api.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetAsync(int Id);
    Task<Category> AddAsync(Category request);
    Task<bool> UpdateAsync(int id, Category request);
    Task<bool> DeleteAsync(int id);
}
