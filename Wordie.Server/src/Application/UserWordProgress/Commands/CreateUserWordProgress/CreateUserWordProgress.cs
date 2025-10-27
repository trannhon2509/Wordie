using MediatR;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.UserWordProgress.Commands.CreateUserWordProgress;

public record CreateUserWordProgressCommand : IRequest<Unit>
{
    public string UserId { get; init; } = string.Empty;
    public int WordCardId { get; init; }
    public int Level { get; init; }
    public DateTime NextReviewAt { get; init; }
}

public class CreateUserWordProgressCommandHandler : IRequestHandler<CreateUserWordProgressCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public CreateUserWordProgressCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(CreateUserWordProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = new global::Wordie.Server.Domain.Entities.UserWordProgress
        {
            UserId = request.UserId,
            WordCardId = request.WordCardId,
            Level = request.Level,
            NextReviewAt = request.NextReviewAt,
            LastReviewedAt = DateTime.UtcNow
        };


    entity.AddDomainEvent(new global::Wordie.Server.Domain.Events.UserWordProgressCreatedEvent(entity));

    _context.UserWordProgresses.Add(entity);

    await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
