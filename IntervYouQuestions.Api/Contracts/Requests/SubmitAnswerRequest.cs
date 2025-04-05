namespace IntervYouQuestions.Api.Contracts.Requests
{
    public class SubmitAnswerRequest
    {
        public string UserId { get; set; }
        public int InterviewId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
        public List<int> SelectedOptionIds { get; set; }
    }
}
