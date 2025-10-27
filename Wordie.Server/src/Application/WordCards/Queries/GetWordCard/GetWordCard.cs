using Microsoft.EntityFrameworkCore;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.WordCards.Queries.GetWordCard;

public record GetWordCardQuery(int Id) : IRequest<WordCardDto>;

public class GetWordCardQueryHandler : IRequestHandler<GetWordCardQuery, WordCardDto>
{
    private readonly IApplicationDbContext _context;

    public GetWordCardQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WordCardDto> Handle(GetWordCardQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.WordCards
            .AsNoTracking()
            .Include(w => w.WordTags)
                .ThenInclude(wt => wt.Tag)
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        return new WordCardDto
        {
            Id = entity!.Id,
            WordSetId = entity.WordSetId,
            Term = entity.Term,
            Definition = entity.Definition,
            Example = entity.Example,
            PartOfSpeech = entity.PartOfSpeech,
            Pronunciation = entity.Pronunciation,
            Note = entity.Note,
            CreatedAt = entity.CreatedAt,
            Tags = entity.WordTags.Select(wt => new TagDto { Id = wt.TagId, Name = wt.Tag?.Name }).ToList()
        };
    }
}

public class WordCardDto
{
    public int Id { get; init; }
    public int WordSetId { get; init; }
    public string? Term { get; init; }
    public string? Definition { get; init; }
    public string? Example { get; init; }
    public string? PartOfSpeech { get; init; }
    public string? Pronunciation { get; init; }
    public string? Note { get; init; }
    public DateTime CreatedAt { get; init; }
    public IReadOnlyCollection<TagDto> Tags { get; init; } = Array.Empty<TagDto>();
}

public class TagDto
{
    public int Id { get; init; }
    public string? Name { get; init; }
}
