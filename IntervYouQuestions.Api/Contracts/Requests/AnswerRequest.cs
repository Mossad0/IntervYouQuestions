namespace IntervYouQuestions.Api.Contracts.Requests
{
    public class AnswerRequest
    {
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
        public string AnswerText { get; set; }
    }
}
