namespace IntervYouQuestions.Api.Contracts.Responses
{
    public class QuestionResultResponse
    {
        public int QuestionId { get; }
        public string QuestionText { get; }
        public bool IsCorrect { get; }
        public string Feedback { get; }

        public QuestionResultResponse(
            int questionId,
            string questionText,
            bool isCorrect,
            string feedback)
        {
            QuestionId = questionId;
            QuestionText = questionText;
            IsCorrect = isCorrect;
            Feedback = feedback;
        }
    }
}

