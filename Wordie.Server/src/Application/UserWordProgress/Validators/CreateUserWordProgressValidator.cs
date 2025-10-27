using FluentValidation;
using Wordie.Server.Application.UserWordProgress.Commands.CreateUserWordProgress;

namespace Wordie.Server.Application.UserWordProgress.Validators;

public class CreateUserWordProgressValidator : AbstractValidator<CreateUserWordProgressCommand>
{
    public CreateUserWordProgressValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WordCardId).GreaterThan(0);
        RuleFor(x => x.Level).GreaterThanOrEqualTo(0);
        RuleFor(x => x.NextReviewAt).GreaterThan(DateTime.MinValue);
    }
}
