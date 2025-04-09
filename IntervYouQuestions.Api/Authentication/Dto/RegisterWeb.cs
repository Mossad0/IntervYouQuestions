namespace IntervYouQuestions.Api.Authentication.Dto;

public record RegisterWeb(

     string FullName,
     string Email,
     //string PhoneNumber,
     string Gender,
     DateOnly DateOfBirth,
     string Password,
     string ConfirmPassword 
);