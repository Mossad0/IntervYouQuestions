using System.ComponentModel.DataAnnotations;

namespace IntervYouQuestions.Api.Entities
{
    public class InterviewQuestion
    {
        public int Id { get; set; }

        [Required]
        public int InterviewId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int OrderIndex { get; set; }

        public virtual Interview Interview { get; set; }
        public virtual Question Question { get; set; }
    }
}
