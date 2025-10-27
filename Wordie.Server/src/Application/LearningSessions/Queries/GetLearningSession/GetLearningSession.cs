using Microsoft.EntityFrameworkCore;
using MediatR;
using Wordie.Server.Application.Common.Interfaces;

namespace Wordie.Server.Application.LearningSessions.Queries.GetLearningSession;

public record GetLearningSessionQuery(int Id) : IRequest<LearningSessionDto>;

public class GetLearningSessionQueryHandler : IRequestHandler<GetLearningSessionQuery, LearningSessionDto>
{
    private readonly IApplicationDbContext _context;

    public GetLearningSessionQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LearningSessionDto> Handle(GetLearningSessionQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.LearningSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (entity == null) return null!;

        return new LearningSessionDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            WordSetId = entity.WordSetId,
            StartedAt = entity.StartedAt,
            EndedAt = entity.EndedAt,
            WordsStudied = entity.WordsStudied,
            CorrectAnswers = entity.CorrectAnswers,
            IncorrectAnswers = entity.IncorrectAnswers
        };
    }
}

public class LearningSessionDto
{
    public int Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public int WordSetId { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? EndedAt { get; init; }
    public int WordsStudied { get; init; }
    public int CorrectAnswers { get; init; }
    public int IncorrectAnswers { get; init; }
}
