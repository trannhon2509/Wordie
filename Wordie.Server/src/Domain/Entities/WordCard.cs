namespace Wordie.Server.Domain.Entities;

public class WordCard : BaseAuditableEntity
{
    // int Id inherited
    public int WordSetId { get; set; }

    public string Term { get; set; } = default!;
    public string Definition { get; set; } = default!;
    public string? Example { get; set; }
    public string? PartOfSpeech { get; set; }
    public string? Pronunciation { get; set; }
    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public WordSet WordSet { get; set; } = default!;
    public ICollection<WordTag> WordTags { get; set; } = new List<WordTag>();
    public ICollection<UserWordProgress> UserProgresses { get; set; } = new List<UserWordProgress>();
}
