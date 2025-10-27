using MediatR;
using Microsoft.Extensions.Logging;
using Wordie.Server.Domain.Events;

namespace Wordie.Server.Infrastructure.Services.DomainEvents;

public class WordSetCreatedEventHandler : INotificationHandler<WordSetCreatedEvent>
{
    private readonly ILogger<WordSetCreatedEventHandler> _logger;

    public WordSetCreatedEventHandler(ILogger<WordSetCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(WordSetCreatedEvent notification, CancellationToken cancellationToken)
    {
        // WordSet uses 'Name' in the domain model
        _logger.LogInformation("Domain event: WordSet created. Id={WordSetId}, Name={Name}",
            notification.WordSet.Id, notification.WordSet.Name);

        return Task.CompletedTask;
    }
}
