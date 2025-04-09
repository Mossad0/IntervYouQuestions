using IntervYouQuestions.Api.Authentication;
using System.ComponentModel.DataAnnotations;

namespace IntervYouQuestions.Api.Entities
{
    public class Interview
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public InterviewStatus Status { get; set; }

        [Required]
        public ExperienceLevel ExperienceLevel { get; set; }


        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string Role { get; set; } // Add this line


        public string UserId { get; set; }

        public virtual AppUser User { get; set; }

        public virtual ICollection<InterviewQuestion> InterviewQuestions { get; set; }
    }
}



