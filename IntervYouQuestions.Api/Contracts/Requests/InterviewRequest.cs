using IntervYouQuestions.Api.Entities;

namespace IntervYouQuestions.Api.Contracts.Requests
{
    public class InterviewRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public InterviewStatus Status { get; set; }
        public ExperienceLevel ExperienceLevel { get; set; }
        public ICollection<InterviewQuestionRequest> Questions { get; set; }
    }
} 