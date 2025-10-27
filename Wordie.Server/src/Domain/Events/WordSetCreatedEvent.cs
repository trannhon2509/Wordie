using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Domain.Events;

public class WordSetCreatedEvent : BaseEvent
{
    public WordSetCreatedEvent(WordSet wordSet)
    {
        WordSet = wordSet;
    }

    public WordSet WordSet { get; }
}
