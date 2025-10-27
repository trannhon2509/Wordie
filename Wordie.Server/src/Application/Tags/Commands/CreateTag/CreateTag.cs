using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.Tags.Commands.CreateTag;

public record CreateTagCommand : IRequest<int>
{
    public string? Name { get; init; }
}

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var entity = new Tag
        {
            Name = request.Name ?? string.Empty
        };

        _context.Tags.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
