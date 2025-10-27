using FluentValidation;
using Wordie.Server.Application.LearningSessions.Commands.CreateLearningSession;

namespace Wordie.Server.Application.LearningSessions.Validators;

public class CreateLearningSessionValidator : AbstractValidator<CreateLearningSessionCommand>
{
    public CreateLearningSessionValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WordSetId).GreaterThan(0);
    }
}
