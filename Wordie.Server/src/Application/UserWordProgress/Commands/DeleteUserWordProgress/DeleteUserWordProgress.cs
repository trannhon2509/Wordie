using MediatR;
using Microsoft.EntityFrameworkCore;
using Wordie.Server.Application.Common.Interfaces;

namespace Wordie.Server.Application.UserWordProgress.Commands.DeleteUserWordProgress;

public record DeleteUserWordProgressCommand(string UserId, int WordCardId) : IRequest<Unit>;

public class DeleteUserWordProgressCommandHandler : IRequestHandler<DeleteUserWordProgressCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteUserWordProgressCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteUserWordProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.UserWordProgresses
            .FirstOrDefaultAsync(u => u.UserId == request.UserId && u.WordCardId == request.WordCardId, cancellationToken: cancellationToken);

        if (entity != null)
        {
            _context.UserWordProgresses.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
