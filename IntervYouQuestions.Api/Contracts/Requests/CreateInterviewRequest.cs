using IntervYouQuestions.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace IntervYouQuestions.Api.Contracts.Requests
{
    public class CreateInterviewRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public ExperienceLevel ExperienceLevel { get; set; }

        [Required]
        public ICollection<int> QuestionIds { get; set; }
    }
}
