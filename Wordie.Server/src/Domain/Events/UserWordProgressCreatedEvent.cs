using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Domain.Events;

public class UserWordProgressCreatedEvent : BaseEvent
{
    public UserWordProgressCreatedEvent(UserWordProgress progress)
    {
        Progress = progress;
    }

    public UserWordProgress Progress { get; }
}
