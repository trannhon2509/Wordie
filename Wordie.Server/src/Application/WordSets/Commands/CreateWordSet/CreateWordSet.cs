using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.WordSets.Commands.CreateWordSet;

public record CreateWordSetCommand : IRequest<int>
{
    public string? Name { get; init; }
}

public class CreateWordSetCommandHandler : IRequestHandler<CreateWordSetCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateWordSetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateWordSetCommand request, CancellationToken cancellationToken)
    {
        var entity = new WordSet();

        // Guard against null name coming from caller
        entity.Name = request.Name ?? string.Empty;

        entity.AddDomainEvent(new global::Wordie.Server.Domain.Events.WordSetCreatedEvent(entity));

        _context.WordSets.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
