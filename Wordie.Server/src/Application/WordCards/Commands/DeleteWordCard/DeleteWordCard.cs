using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.WordCards.Commands.DeleteWordCard;

public record DeleteWordCardCommand(int Id) : IRequest;

public class DeleteWordCardCommandHandler : IRequestHandler<DeleteWordCardCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteWordCardCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteWordCardCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WordCards.FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.WordCards.Remove(entity!);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
