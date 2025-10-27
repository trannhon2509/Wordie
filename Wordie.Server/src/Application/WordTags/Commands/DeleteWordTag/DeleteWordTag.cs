using MediatR;
using Wordie.Server.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Wordie.Server.Application.WordTags.Commands.DeleteWordTag;

public record DeleteWordTagCommand(int WordCardId, int TagId) : IRequest<Unit>;

public class DeleteWordTagCommandHandler : IRequestHandler<DeleteWordTagCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteWordTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteWordTagCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WordTags.FindAsync(new object[] { request.WordCardId, request.TagId }, cancellationToken);
        if (entity != null)
        {
            _context.WordTags.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
