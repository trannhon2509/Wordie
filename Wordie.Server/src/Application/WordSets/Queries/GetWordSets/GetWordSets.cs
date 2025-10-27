using Microsoft.EntityFrameworkCore;
using MediatR;
using Wordie.Server.Application.Common.Interfaces;

namespace Wordie.Server.Application.WordSets.Queries.GetWordSets;

public record GetWordSetsQuery : IRequest<List<WordSetDto>>;

public class GetWordSetsQueryHandler : IRequestHandler<GetWordSetsQuery, List<WordSetDto>>
{
    private readonly IApplicationDbContext _context;

    public GetWordSetsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WordSetDto>> Handle(GetWordSetsQuery request, CancellationToken cancellationToken)
    {
        return await _context.WordSets
            .AsNoTracking()
            .Select(ws => new WordSetDto { Id = ws.Id, Name = ws.Name })
            .ToListAsync(cancellationToken);
    }
}

public class WordSetDto
{
    public int Id { get; init; }
    public string? Name { get; init; }
}
