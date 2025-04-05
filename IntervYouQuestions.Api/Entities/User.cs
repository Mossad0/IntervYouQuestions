using System.ComponentModel.DataAnnotations;

namespace IntervYouQuestions.Api.Entities
{
    public class User
    {
        public string UserId { get; set; }
       
        public string Username { get; set; }
       
        public string Email { get; set; }
        public string Password { get; set; } // In a real-world app, this would be a hash
        public string ExperienceLevel { get; set; } // Fresh, Junior, Mid, Senior
        public string Role { get; set; } // Backend, Frontend, Mobile, AI
        public DateTime RegisteredDate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    }
}
