using FluentValidation;
using Wordie.Server.Application.WordSets.Commands.CreateWordSet;

namespace Wordie.Server.Application.WordSets.Validators;

public class CreateWordSetValidator : AbstractValidator<CreateWordSetCommand>
{
    public CreateWordSetValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
    }
}
