using Wordie.Server.Application.Common.Interfaces;

namespace Wordie.Server.Application.WordSets.Commands.DeleteWordSet;

public record DeleteWordSetCommand(int Id) : IRequest;

public class DeleteWordSetCommandHandler : IRequestHandler<DeleteWordSetCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteWordSetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteWordSetCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WordSets
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.WordSets.Remove(entity!);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
