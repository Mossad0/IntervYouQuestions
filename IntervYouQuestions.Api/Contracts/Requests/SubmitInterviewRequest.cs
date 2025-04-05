using System.ComponentModel.DataAnnotations;

namespace IntervYouQuestions.Api.Contracts.Requests
{
    public class SubmitInterviewRequest
    {
        [Required]
        public string InterviewId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public List<AnswerRequest> Answers { get; set; }

        public int TimeTakenInMinutes { get; set; }
    }
}
