using FluentValidation;
using Wordie.Server.Application.LearningSessions.Commands.UpdateLearningSession;

namespace Wordie.Server.Application.LearningSessions.Validators;

public class UpdateLearningSessionValidator : AbstractValidator<UpdateLearningSessionCommand>
{
    public UpdateLearningSessionValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
