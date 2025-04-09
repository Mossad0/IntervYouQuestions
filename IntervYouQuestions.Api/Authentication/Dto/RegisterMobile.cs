namespace IntervYouQuestions.Api.Authentication.Dto;

public record RegisterMobile(
    string FullName,
    string Email ,
    string Password, 
    string ConfirmPassword
    
    );
