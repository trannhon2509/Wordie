using MediatR;
using Microsoft.Extensions.Logging;
using Wordie.Server.Domain.Events;

namespace Wordie.Server.Infrastructure.Services.DomainEvents;

public class WordCardCreatedEventHandler : INotificationHandler<WordCardCreatedEvent>
{
    private readonly ILogger<WordCardCreatedEventHandler> _logger;

    public WordCardCreatedEventHandler(ILogger<WordCardCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(WordCardCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain event: WordCard created. Id={WordCardId}, Term={Term}",
            notification.WordCard.Id, notification.WordCard.Term);

        // Small, safe handler: just log. Extend to perform side-effects (notifications, read-model updates) as needed.
        return Task.CompletedTask;
    }
}
