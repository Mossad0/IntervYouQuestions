namespace IntervYouQuestions.Api.Entities;

public partial class Question
{
    public int QuestionId { get; set; }

    public string Type { get; set; } = null!;

    public string Text { get; set; } = null!;

    public string Difficulty { get; set; } = null!;

    public int TopicId { get; set; }

    public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();

    public virtual ICollection<ModelAnswer> ModelAnswers { get; set; } = new List<ModelAnswer>();

    public virtual Topic Topic { get; set; } = null!;
    public virtual ICollection<InterviewQuestion> InterviewQuestions { get; set; } = new List<InterviewQuestion>();
}
