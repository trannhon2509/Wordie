using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.WordCards.Commands.UpdateWordCard;

public record UpdateWordCardCommand : IRequest
{
    public int Id { get; init; }
    public string? Term { get; init; }
    public string? Definition { get; init; }
    public string? Example { get; init; }
    public string? PartOfSpeech { get; init; }
    public string? Pronunciation { get; init; }
    public string? Note { get; init; }
}

public class UpdateWordCardCommandHandler : IRequestHandler<UpdateWordCardCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateWordCardCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateWordCardCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WordCards.FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        if (request.Term is not null) entity.Term = request.Term;
        if (request.Definition is not null) entity.Definition = request.Definition;
        if (request.Example is not null) entity.Example = request.Example;
        if (request.PartOfSpeech is not null) entity.PartOfSpeech = request.PartOfSpeech;
        if (request.Pronunciation is not null) entity.Pronunciation = request.Pronunciation;
        if (request.Note is not null) entity.Note = request.Note;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
