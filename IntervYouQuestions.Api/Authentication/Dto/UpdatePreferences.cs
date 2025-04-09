namespace IntervYouQuestions.Api.Authentication.Dto;

public record UpdatePreferences(
    string PreferredRole,
    string ExperienceLevel,
    string DailyStudyHours
);
