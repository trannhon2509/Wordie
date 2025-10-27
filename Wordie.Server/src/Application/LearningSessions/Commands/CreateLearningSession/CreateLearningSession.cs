using MediatR;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.LearningSessions.Commands.CreateLearningSession;

public record CreateLearningSessionCommand : IRequest<int>
{
    public string UserId { get; init; } = string.Empty;
    public int WordSetId { get; init; }
}

public class CreateLearningSessionCommandHandler : IRequestHandler<CreateLearningSessionCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateLearningSessionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateLearningSessionCommand request, CancellationToken cancellationToken)
    {
        var entity = new LearningSession
        {
            UserId = request.UserId,
            WordSetId = request.WordSetId,
            StartedAt = DateTime.UtcNow
        };

        entity.AddDomainEvent(new global::Wordie.Server.Domain.Events.LearningSessionCreatedEvent(entity));

        _context.LearningSessions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
