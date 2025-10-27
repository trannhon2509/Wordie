using FluentValidation;
using Wordie.Server.Application.WordCards.Commands.CreateWordCard;

namespace Wordie.Server.Application.WordCards.Validators;

public class CreateWordCardValidator : AbstractValidator<CreateWordCardCommand>
{
    public CreateWordCardValidator()
    {
        RuleFor(x => x.WordSetId).GreaterThan(0);
        RuleFor(x => x.Term).NotEmpty().WithMessage("Term is required");
        RuleFor(x => x.Definition).NotEmpty().WithMessage("Definition is required");
    }
}
