namespace IntervYouQuestions.Api.Entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Weight { get; set; }

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
