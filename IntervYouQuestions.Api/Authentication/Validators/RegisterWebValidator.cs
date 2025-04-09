namespace IntervYouQuestions.Api.Authentication.Validators;

using FluentValidation;
using IntervYouQuestions.Api.Authentication.Dto;

public class RegisterWebValidator : AbstractValidator<RegisterWeb>
{
    public RegisterWebValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        //RuleFor(x => x.PhoneNumber)
        //    .NotEmpty().WithMessage("Phone number is required");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .Must(g => g == "Male" || g == "Female")
            .WithMessage("Gender must be Male or Female");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .Must(dob => dob <= DateOnly.FromDateTime(DateTime.Today.AddYears(-18)))
            .WithMessage("You must be at least 18 years old");

        RuleFor(x => x.Password)
    .NotEmpty().WithMessage("Password is required")
    .MinimumLength(8).WithMessage("Password must be at least 8 characters")
    .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
    .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
    .Matches(@"[0-9]").WithMessage("Password must contain at least one digit")
    .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one non-alphanumeric character");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}

