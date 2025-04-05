using FluentValidation;

namespace IntervYouQuestions.Api.Contracts.Validations
{
    public class UpdateInterviewDtoValidator : AbstractValidator<UpdateInterviewRequest>
    {
        public UpdateInterviewDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.ExpirationDate)
                .NotEmpty()
                .Must(date => date > DateTime.UtcNow)
                .WithMessage("Expiration date must be in the future");

            RuleFor(x => x.ExperienceLevel)
                .IsInEnum()
                .WithMessage("Invalid experience level");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid interview status");

            RuleFor(x => x.QuestionIds)
                .NotEmpty()
                .WithMessage("At least one question is required");
        }
    }
} 