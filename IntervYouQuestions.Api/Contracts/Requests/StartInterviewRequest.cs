using IntervYouQuestions.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace IntervYouQuestions.Api.Contracts.Requests
{
    public class StartInterviewRequest
    {
        [Required]
        public ExperienceLevel ExperienceLevel { get; set; }

        [Required]
        public RoleType Role { get; set; }

        [Range(15, 120)]
        public int TimeLimitInMinutes { get; set; } = 30;

        [Range(5, 50)]
        public int NumberOfQuestions { get; set; } = 10;

        // Added user ID property
        public string UserId { get; set; }
    }
}