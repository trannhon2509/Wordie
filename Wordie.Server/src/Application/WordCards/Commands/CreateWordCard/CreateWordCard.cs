using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.WordCards.Commands.CreateWordCard;

public record CreateWordCardCommand : IRequest<int>
{
    public int WordSetId { get; init; }
    public string? Term { get; init; }
    public string? Definition { get; init; }
    public string? Example { get; init; }
    public string? PartOfSpeech { get; init; }
    public string? Pronunciation { get; init; }
    public string? Note { get; init; }
}

public class CreateWordCardCommandHandler : IRequestHandler<CreateWordCardCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateWordCardCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateWordCardCommand request, CancellationToken cancellationToken)
    {
        var entity = new WordCard
        {
            WordSetId = request.WordSetId,
            Term = request.Term ?? string.Empty,
            Definition = request.Definition ?? string.Empty,
            Example = request.Example,
            PartOfSpeech = request.PartOfSpeech,
            Pronunciation = request.Pronunciation,
            Note = request.Note
        };

        entity.AddDomainEvent(new global::Wordie.Server.Domain.Events.WordCardCreatedEvent(entity));

        _context.WordCards.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
