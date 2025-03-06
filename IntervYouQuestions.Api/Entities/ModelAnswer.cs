using System;
using System.Collections.Generic;

namespace IntervYouQuestions.Api.Entities;

public partial class ModelAnswer
{
    public int ModelAnswerId { get; set; }

    public string Text { get; set; } = null!;

    public string KeyPoints { get; set; } = null!;

    public int QuestionId { get; set; }

    public virtual Question Question { get; set; } = null!;
}
