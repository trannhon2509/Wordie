namespace Wordie.Server.Domain.Entities;
public class WordSynonym
{
    public int WordCardId { get; set; }
    public int SynonymCardId { get; set; }

    // Store creator as string user id (keeps Domain independent of Infrastructure)
    public string CreatorId { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public WordCard WordCard { get; set; } = default!;
    public WordCard SynonymCard { get; set; } = default!;
}
