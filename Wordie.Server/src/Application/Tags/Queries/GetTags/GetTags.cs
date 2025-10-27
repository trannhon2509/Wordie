using Microsoft.EntityFrameworkCore;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.Tags.Queries.GetTags;

public record GetTagsQuery : IRequest<TagsVm>;

public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, TagsVm>
{
    private readonly IApplicationDbContext _context;

    public GetTagsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TagsVm> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _context.Tags.AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new TagDto { Id = t.Id, Name = t.Name })
            .ToListAsync(cancellationToken);

        return new TagsVm { Tags = tags };
    }
}

public class TagsVm
{
    public IReadOnlyCollection<TagDto> Tags { get; init; } = Array.Empty<TagDto>();
}

public class TagDto
{
    public int Id { get; init; }
    public string? Name { get; init; }
}
