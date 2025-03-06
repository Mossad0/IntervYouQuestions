namespace IntervYouQuestions.Api.Services;

public class TopicService(InterviewModuleContext context) : ITopicService
{
    private readonly InterviewModuleContext _context = context;
    public async Task<IEnumerable<Topic>> GetAllAsync()
    {
        return await _context.Topics.Include(t => t.Category).AsNoTracking().ToListAsync();
    }
    public async Task<Topic?> GetAsync(int Id)
    {
        return await _context.Topics.Include(t => t.Category).AsNoTracking().FirstOrDefaultAsync(t => t.TopicId == Id);
    }

    public async Task<Topic> AddAsync(Topic request)
    {
        await _context.AddAsync(request);
        await _context.SaveChangesAsync();
        return request;
    }
    public async Task<bool> UpdateAsync(int id, Topic request)
    {
        var updated = await _context.Topics.FindAsync(id); 
        if (updated is null) return false;
        updated.Name = request.Name;
        updated.CategoryId = request.CategoryId;
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
