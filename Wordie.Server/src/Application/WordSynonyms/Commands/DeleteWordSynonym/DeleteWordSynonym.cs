using MediatR;
using Wordie.Server.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Wordie.Server.Application.WordSynonyms.Commands.DeleteWordSynonym;

public record DeleteWordSynonymCommand(int WordCardId, int SynonymCardId) : IRequest<Unit>;

public class DeleteWordSynonymCommandHandler : IRequestHandler<DeleteWordSynonymCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteWordSynonymCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteWordSynonymCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WordSynonyms.FirstOrDefaultAsync(w => w.WordCardId == request.WordCardId && w.SynonymCardId == request.SynonymCardId, cancellationToken);
        if (entity != null)
        {
            _context.WordSynonyms.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
