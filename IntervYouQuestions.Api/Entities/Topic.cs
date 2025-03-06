namespace IntervYouQuestions.Api.Entities;

public partial class Topic
{
    public int TopicId { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
