using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.Tags.Commands.DeleteTag;

public record DeleteTagCommand(int Id) : IRequest;

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tags.FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.Tags.Remove(entity!);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
