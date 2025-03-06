namespace IntervYouQuestions.Api.Services;

public class QuestionService : IQuestionService
{
    private readonly InterviewModuleContext _context;

    public QuestionService(InterviewModuleContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Question>> GetAllAsync()
    {
        return await _context.Questions
            .Include(q => q.Topic)
            .ThenInclude(t => t.Category)
            .Include(q => q.QuestionOptions)
            .Include(q => q.ModelAnswers)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<QuestionResponse?> GetAsync(int id)
    {
        var question = await _context.Questions
            .Include(q => q.QuestionOptions)
            .Include(q => q.ModelAnswers)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.QuestionId == id);

        if (question == null)
            return null;

        return new QuestionResponse(
            question.QuestionId,
            question.Type,
            question.Text,
            question.Difficulty,
            question.TopicId,
            question.QuestionOptions.Select(o => new QuestionOptionResponse(o.OptionId, o.Text, o.IsCorrect)).ToList(),
            question.ModelAnswers.Select(ma => new ModelAnswerResponse(ma.ModelAnswerId, ma.Text, ma.KeyPoints)).ToList()
        );
    }

    public async Task<QuestionResponse> AddAsync(Question request)
    {
        var question = new Question
        {
            Type = request.Type,
            Text = request.Text,
            Difficulty = request.Difficulty,
            TopicId = request.TopicId,
            QuestionOptions = request.QuestionOptions.Select(o => new QuestionOption
            {
                Text = o.Text,
                IsCorrect = o.IsCorrect
            }).ToList(),
            ModelAnswers = request.ModelAnswers.Select(ma => new ModelAnswer
            {
                Text = ma.Text,
                KeyPoints = ma.KeyPoints
            }).ToList()
        };

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        // Ensure that the QuestionOptions are linked to the question
        foreach (var option in question.QuestionOptions)
        {
            option.QuestionId = question.QuestionId;
        }

        await _context.SaveChangesAsync();

        return new QuestionResponse(
            question.QuestionId,
            question.Type,
            question.Text,
            question.Difficulty,
            question.TopicId,
            question.QuestionOptions.Select(o => new QuestionOptionResponse(o.OptionId, o.Text, o.IsCorrect)).ToList(),
            question.ModelAnswers.Select(ma => new ModelAnswerResponse(ma.ModelAnswerId, ma.Text, ma.KeyPoints)).ToList()
        );
    }


    public async Task<Question> AddWithOptionAsync(QuestionWithOptionsRequest request)
    {
        var question = new Question
        {
            Type = request.Type,
            Text = request.Text,
            Difficulty = request.Difficulty,
            TopicId = request.TopicId,
            QuestionOptions = request.Options.Select(o => new QuestionOption
            {
                Text = o.Text,
                IsCorrect = o.IsCorrect
            }).ToList()
        };

        await _context.Questions.AddAsync(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task<Question> AddWithModelAnswerAsync(QuestionWithModelAnswerRequest request)
    {
        var question = new Question
        {
            Type = request.Type,
            Text = request.Text,
            Difficulty = request.Difficulty,
            TopicId = request.TopicId,
            ModelAnswers = request.ModelAnswers.Select(ma => new ModelAnswer
            {
                Text = ma.Text,
                KeyPoints = ma.KeyPoints
            }).ToList()
        };

        await _context.Questions.AddAsync(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task<bool> UpdateAsync(int id, Question request)
    {
        var existingQuestion = await _context.Questions.FindAsync(id);
        if (existingQuestion is null) return false;

        existingQuestion.Type = request.Type;
        existingQuestion.Text = request.Text;
        existingQuestion.Difficulty = request.Difficulty;
        existingQuestion.TopicId = request.TopicId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        if (question is null) return false;

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();
        return true;
    }
}
