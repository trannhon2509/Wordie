using MediatR;
using Microsoft.Extensions.Logging;
using Wordie.Server.Domain.Events;

namespace Wordie.Server.Infrastructure.Services.DomainEvents;

public class LearningSessionCreatedEventHandler : INotificationHandler<LearningSessionCreatedEvent>
{
    private readonly ILogger<LearningSessionCreatedEventHandler> _logger;

    public LearningSessionCreatedEventHandler(ILogger<LearningSessionCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(LearningSessionCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain event: LearningSession created. Id={SessionId}, UserId={UserId}, StartedAt={StartedAt}",
            notification.Session.Id, notification.Session.UserId, notification.Session.StartedAt);

        return Task.CompletedTask;
    }
}
