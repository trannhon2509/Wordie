using MediatR;
using Microsoft.Extensions.Logging;
using Wordie.Server.Domain.Events;

namespace Wordie.Server.Infrastructure.Services.DomainEvents;

public class UserWordProgressCreatedEventHandler : INotificationHandler<UserWordProgressCreatedEvent>
{
    private readonly ILogger<UserWordProgressCreatedEventHandler> _logger;

    public UserWordProgressCreatedEventHandler(ILogger<UserWordProgressCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserWordProgressCreatedEvent notification, CancellationToken cancellationToken)
    {
        var p = notification.Progress;
        _logger.LogInformation("Domain event: UserWordProgress created. Id={Id}, UserId={UserId}, WordCardId={CardId}",
            p.Id, p.UserId, p.WordCardId);

        return Task.CompletedTask;
    }
}
