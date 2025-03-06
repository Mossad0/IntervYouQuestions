
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using static System.Net.Mime.MediaTypeNames;

namespace IntervYouQuestions.Api.Services;

public class CategoryService(InterviewModuleContext context) : ICategoryService
{
    private readonly InterviewModuleContext _context = context;
    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
        .Include(c => c.Topics)
        .AsNoTracking()
        .ToListAsync();
    }
    public async Task<Category?> GetAsync(int Id)
    {
        return await _context.Categories
        .Include(c => c.Topics)
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.CategoryId == Id);
    }

    public async Task<Category> AddAsync(Category request)
    {
        await _context.AddAsync(request);
        await _context.SaveChangesAsync();
        return request;
    }
    public async Task<bool> UpdateAsync(int id, Category request)
    {
        var updated = await GetAsync(id);
        if (updated is null) return false;
        updated.Name = request.Name;
        updated.Weight = request.Weight;
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var deleted = await GetAsync(id);
        if (deleted is null) return false;
        _context.Remove(deleted);
        await _context.SaveChangesAsync();
        return true;

    }

   
}
