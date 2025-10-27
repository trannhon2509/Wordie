using MediatR;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.WordTags.Commands.CreateWordTag;

public record CreateWordTagCommand(int WordCardId, int TagId) : IRequest<Unit>;

public class CreateWordTagCommandHandler : IRequestHandler<CreateWordTagCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public CreateWordTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(CreateWordTagCommand request, CancellationToken cancellationToken)
    {
        var entity = new WordTag
        {
            WordCardId = request.WordCardId,
            TagId = request.TagId
        };

        _context.WordTags.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
