namespace IntervYouQuestions.Api.Entities;

public partial class QuestionOption
{
    public int OptionId { get; set; }

    public string Text { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }

    public virtual Question Question { get; set; } = null!;
}
