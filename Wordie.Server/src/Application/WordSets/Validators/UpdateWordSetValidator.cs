using FluentValidation;
using Wordie.Server.Application.WordSets.Commands.UpdateWordSet;

namespace Wordie.Server.Application.WordSets.Validators;

public class UpdateWordSetValidator : AbstractValidator<UpdateWordSetCommand>
{
    public UpdateWordSetValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().When(x => x.Name is not null);
    }
}
