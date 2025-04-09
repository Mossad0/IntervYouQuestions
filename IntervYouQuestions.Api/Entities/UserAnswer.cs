using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntervYouQuestions.Api.Entities
{
    public class UserAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int InterviewId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        public string AnswerText { get; set; }

        public string SelectedOptions { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [ForeignKey("InterviewId")]
        public Interview Interview { get; set; }

        [ForeignKey("QuestionId")]
        public Question Question { get; set; }
    }
}