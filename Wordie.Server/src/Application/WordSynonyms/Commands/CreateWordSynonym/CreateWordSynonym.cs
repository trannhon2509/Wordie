using MediatR;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.WordSynonyms.Commands.CreateWordSynonym;

public record CreateWordSynonymCommand(int WordCardId, int SynonymCardId, string CreatorId) : IRequest<Unit>;

public class CreateWordSynonymCommandHandler : IRequestHandler<CreateWordSynonymCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public CreateWordSynonymCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(CreateWordSynonymCommand request, CancellationToken cancellationToken)
    {
        var entity = new WordSynonym
        {
            WordCardId = request.WordCardId,
            SynonymCardId = request.SynonymCardId,
            CreatorId = request.CreatorId,
            CreatedAt = DateTime.UtcNow
        };

        _context.WordSynonyms.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
