using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Domain.Events;

public class LearningSessionCreatedEvent : BaseEvent
{
    public LearningSessionCreatedEvent(LearningSession session)
    {
        Session = session;
    }

    public LearningSession Session { get; }
}
