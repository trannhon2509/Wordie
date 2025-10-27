using Wordie.Server.Application.Common.Interfaces;

namespace Wordie.Server.Application.WordSets.Commands.PurgeWordSets;

public record PurgeWordSetsCommand : IRequest;

public class PurgeWordSetsCommandHandler : IRequestHandler<PurgeWordSetsCommand>
{
    private readonly IApplicationDbContext _context;

    public PurgeWordSetsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(PurgeWordSetsCommand request, CancellationToken cancellationToken)
    {
        _context.WordSets.RemoveRange(_context.WordSets);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
