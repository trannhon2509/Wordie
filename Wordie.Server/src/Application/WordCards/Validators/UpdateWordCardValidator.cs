using FluentValidation;
using Wordie.Server.Application.WordCards.Commands.UpdateWordCard;

namespace Wordie.Server.Application.WordCards.Validators;

public class UpdateWordCardValidator : AbstractValidator<UpdateWordCardCommand>
{
    public UpdateWordCardValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Term).NotEmpty().When(x => x.Term is not null);
        RuleFor(x => x.Definition).NotEmpty().When(x => x.Definition is not null);
    }
}
