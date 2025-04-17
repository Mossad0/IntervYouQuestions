namespace IntervYouQuestions.Api.Contracts.Requests;

public class ExternalAnswerRequest
{
    public string model_answer { get; set; } = string.Empty!;

    public string student_answer { get; set; } = string.Empty!;
}
