namespace IntervYouQuestions.Api.Authentication.Dto;

public record ResetPasswordDto(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword
);