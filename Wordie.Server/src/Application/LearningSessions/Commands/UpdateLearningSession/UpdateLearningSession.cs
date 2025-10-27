using MediatR;
using Wordie.Server.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.LearningSessions.Commands.UpdateLearningSession;

public record UpdateLearningSessionCommand : IRequest<Unit>
{
    public int Id { get; init; }
    public DateTime? EndedAt { get; init; }
    public int WordsStudied { get; init; }
    public int CorrectAnswers { get; init; }
    public int IncorrectAnswers { get; init; }
}

public class UpdateLearningSessionCommandHandler : IRequestHandler<UpdateLearningSessionCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateLearningSessionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateLearningSessionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.LearningSessions.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        if (entity == null) throw new KeyNotFoundException("LearningSession not found");

        entity.EndedAt = request.EndedAt;
        entity.WordsStudied = request.WordsStudied;
        entity.CorrectAnswers = request.CorrectAnswers;
        entity.IncorrectAnswers = request.IncorrectAnswers;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
