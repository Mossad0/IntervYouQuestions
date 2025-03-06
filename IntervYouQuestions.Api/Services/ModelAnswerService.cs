namespace IntervYouQuestions.Api.Services;

public class ModelAnswerService(InterviewModuleContext context) : IModelAnswerService
{
    private readonly InterviewModuleContext _context = context;

    public async Task<IEnumerable<ModelAnswer>> GetAllAsync()
    {
        return await _context.ModelAnswers
            .Include(ma => ma.Question)
            .ThenInclude(q=> q.Topic)
            .ThenInclude(c => c.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ModelAnswer?> GetAsync(int id)
    {
        return await _context.ModelAnswers
            .Include(ma => ma.Question)
            .ThenInclude(q => q.Topic)
            .ThenInclude(c => c.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(ma => ma.ModelAnswerId == id);
    }

    //public async Task<IEnumerable<ModelAnswer>> GetByQuestionIdAsync(int questionId)
    //{
    //    return await _context.ModelAnswers
    //        .Where(ma => ma.QuestionId == questionId)
    //        .Include(ma => ma.Question)
    //        .AsNoTracking()
    //        .ToListAsync();
    //}

    public async Task<ModelAnswer> AddAsync(ModelAnswer request)
    {
        await _context.ModelAnswers.AddAsync(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task<bool> UpdateAsync(int id, ModelAnswer request)
    {
        var existingAnswer = await _context.ModelAnswers.FindAsync(id);
        if (existingAnswer is null) return false;

        existingAnswer.Text = request.Text;
        existingAnswer.KeyPoints = request.KeyPoints;
        existingAnswer.QuestionId = request.QuestionId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var answer = await _context.ModelAnswers.FindAsync(id);
        if (answer is null) return false;

        _context.ModelAnswers.Remove(answer);
        await _context.SaveChangesAsync();
        return true;
    }
}
