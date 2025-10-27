namespace Wordie.Server.Domain.Events;

public class WordCardCreatedEvent : BaseEvent
{
	public WordCardCreatedEvent(WordCard wordCard)
	{
		WordCard = wordCard;
	}

	public WordCard WordCard { get; }
}

