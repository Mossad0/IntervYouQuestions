namespace IntervYouQuestions.Api.Services;

public class QuestionOptionService(InterviewModuleContext context) : IQuestionOptionService
{
    private readonly InterviewModuleContext _context = context;

    public async Task<IEnumerable<QuestionOption>> GetAllAsync()
    {
        return await _context.QuestionOptions
            .Include(qo => qo.Question)
            .ThenInclude(q=> q.Topic)
            .ThenInclude(t=> t.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<QuestionOption?> GetAsync(int id)
    {
        return await _context.QuestionOptions
            .Include(qo => qo.Question)
            .ThenInclude(q => q.Topic)
            .ThenInclude(t => t.Category)
            .FirstOrDefaultAsync(qo => qo.OptionId == id);
    }

    public async Task<QuestionOption> AddAsync(QuestionOption request)
    {
        await _context.QuestionOptions.AddAsync(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task<bool> UpdateAsync(int id, QuestionOption request)
    {
        var existingOption = await _context.QuestionOptions.FindAsync(id);
        if (existingOption is null) return false;

        existingOption.Text = request.Text;
        existingOption.IsCorrect = request.IsCorrect;
        existingOption.QuestionId = request.QuestionId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var option = await _context.QuestionOptions.FindAsync(id);
        if (option is null) return false;

        _context.QuestionOptions.Remove(option);
        await _context.SaveChangesAsync();
        return true;
    }
}
