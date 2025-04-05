using IntervYouQuestions.Api.Entities;

namespace IntervYouQuestions.Api.Contracts.Requests
{
    public class InterviewQuestionRequest
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int OrderIndex { get; set; }
        public Question QuestionType { get; set; }
        public ICollection<QuestionOption> Options { get; set; }
    }
} 