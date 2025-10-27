using Microsoft.EntityFrameworkCore;
using MediatR;
using Wordie.Server.Application.Common.Interfaces;

namespace Wordie.Server.Application.LearningSessions.Queries.GetLearningSessionList;

public record GetLearningSessionListQuery(string UserId) : IRequest<List<LearningSessionListItemDto>>;

public class GetLearningSessionListQueryHandler : IRequestHandler<GetLearningSessionListQuery, List<LearningSessionListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLearningSessionListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LearningSessionListItemDto>> Handle(GetLearningSessionListQuery request, CancellationToken cancellationToken)
    {
        return await _context.LearningSessions
            .AsNoTracking()
            .Where(l => l.UserId == request.UserId)
            .Select(l => new LearningSessionListItemDto
            {
                Id = l.Id,
                WordSetId = l.WordSetId,
                StartedAt = l.StartedAt,
                EndedAt = l.EndedAt,
                WordsStudied = l.WordsStudied
            })
            .ToListAsync(cancellationToken);
    }
}

public class LearningSessionListItemDto
{
    public int Id { get; init; }
    public int WordSetId { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? EndedAt { get; init; }
    public int WordsStudied { get; init; }
}
