using Microsoft.EntityFrameworkCore;
using MediatR;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.UserWordProgress.Queries.GetUserWordProgress;

public record GetUserWordProgressQuery(string UserId, int WordCardId) : IRequest<UserWordProgressDto>;

public class GetUserWordProgressQueryHandler : IRequestHandler<GetUserWordProgressQuery, UserWordProgressDto>
{
    private readonly IApplicationDbContext _context;

    public GetUserWordProgressQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserWordProgressDto> Handle(GetUserWordProgressQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.UserWordProgresses
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == request.UserId && u.WordCardId == request.WordCardId, cancellationToken);

        if (entity == null) return null!;

        return new UserWordProgressDto
        {
            UserId = entity.UserId,
            WordCardId = entity.WordCardId,
            Level = entity.Level,
            NextReviewAt = entity.NextReviewAt,
            LastReviewedAt = entity.LastReviewedAt,
            CorrectCount = entity.CorrectCount,
            IncorrectCount = entity.IncorrectCount
        };
    }
}

public class UserWordProgressDto
{
    public string UserId { get; init; } = string.Empty;
    public int WordCardId { get; init; }
    public int Level { get; init; }
    public DateTime NextReviewAt { get; init; }
    public DateTime LastReviewedAt { get; init; }
    public int CorrectCount { get; init; }
    public int IncorrectCount { get; init; }
}
