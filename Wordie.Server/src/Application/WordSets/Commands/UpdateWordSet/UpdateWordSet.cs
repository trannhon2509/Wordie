using Wordie.Server.Application.Common.Interfaces;

namespace Wordie.Server.Application.WordSets.Commands.UpdateWordSet;

public record UpdateWordSetCommand : IRequest
{
    public int Id { get; init; }

    public string? Name { get; init; }
}

public class UpdateWordSetCommandHandler : IRequestHandler<UpdateWordSetCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateWordSetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateWordSetCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WordSets
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        if (request.Name is not null)
        {
            entity.Name = request.Name;
        }

        await _context.SaveChangesAsync(cancellationToken);

    }
}
