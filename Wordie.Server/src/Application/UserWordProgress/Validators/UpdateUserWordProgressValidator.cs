using FluentValidation;
using Wordie.Server.Application.UserWordProgress.Commands.UpdateUserWordProgress;

namespace Wordie.Server.Application.UserWordProgress.Validators;

public class UpdateUserWordProgressValidator : AbstractValidator<UpdateUserWordProgressCommand>
{
    public UpdateUserWordProgressValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WordCardId).GreaterThan(0);
        RuleFor(x => x.Level).GreaterThanOrEqualTo(0);
    }
}
