using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IntervYouQuestions.Api.Authentication;

public class AppUser : IdentityUser
{
    [Required] // Make FullName required if it always should be
    [StringLength(150, MinimumLength = 6)] // Example: Max 150, Min 2 chars
    public string FullName { get; set; }

    [StringLength(10)] // Example max length
    public string? Gender { get; set; }
   

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(100)]
    public string? PreferredRole { get; set; }        // e.g., Backend, Frontend

    [StringLength(100)]
    public string? ExperienceLevel { get; set; }      // e.g., Fresh Graduate, Junior

    [StringLength(50)]
    public string? DailyStudyHours { get; set; }      // One of the 4 options
    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
