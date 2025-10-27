using MediatR;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Wordie.Server.Application.UserWordProgress.Commands.UpdateUserWordProgress;

public record UpdateUserWordProgressCommand : IRequest<Unit>
{
    public string UserId { get; init; } = string.Empty;
    public int WordCardId { get; init; }
    public int Level { get; init; }
    public DateTime NextReviewAt { get; init; }
    public int CorrectCount { get; init; }
    public int IncorrectCount { get; init; }
}

public class UpdateUserWordProgressCommandHandler : IRequestHandler<UpdateUserWordProgressCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserWordProgressCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateUserWordProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.UserWordProgresses
            .FirstOrDefaultAsync(u => u.UserId == request.UserId && u.WordCardId == request.WordCardId, cancellationToken);

        if (entity == null) throw new KeyNotFoundException("UserWordProgress not found");

        entity.Level = request.Level;
        entity.NextReviewAt = request.NextReviewAt;
        entity.CorrectCount = request.CorrectCount;
        entity.IncorrectCount = request.IncorrectCount;
        entity.LastReviewedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
