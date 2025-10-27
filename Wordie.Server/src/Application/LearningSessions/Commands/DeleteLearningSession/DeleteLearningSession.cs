using MediatR;
using Wordie.Server.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Wordie.Server.Application.LearningSessions.Commands.DeleteLearningSession;

public record DeleteLearningSessionCommand(int Id) : IRequest<Unit>;

public class DeleteLearningSessionCommandHandler : IRequestHandler<DeleteLearningSessionCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteLearningSessionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteLearningSessionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.LearningSessions.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        if (entity != null)
        {
            _context.LearningSessions.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
