using Microsoft.EntityFrameworkCore;
using MediatR;
using Wordie.Server.Application.Common.Interfaces;

namespace Wordie.Server.Application.UserWordProgress.Queries.GetUserWordProgressList;

public record GetUserWordProgressListQuery(string UserId) : IRequest<List<UserWordProgressListItemDto>>;

public class GetUserWordProgressListQueryHandler : IRequestHandler<GetUserWordProgressListQuery, List<UserWordProgressListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUserWordProgressListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserWordProgressListItemDto>> Handle(GetUserWordProgressListQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.UserWordProgresses
            .AsNoTracking()
            .Where(u => u.UserId == request.UserId)
            .Select(u => new UserWordProgressListItemDto
            {
                WordCardId = u.WordCardId,
                Level = u.Level,
                NextReviewAt = u.NextReviewAt,
                LastReviewedAt = u.LastReviewedAt
            })
            .ToListAsync(cancellationToken);

        return items;
    }
}

public class UserWordProgressListItemDto
{
    public int WordCardId { get; init; }
    public int Level { get; init; }
    public DateTime NextReviewAt { get; init; }
    public DateTime LastReviewedAt { get; init; }
}
