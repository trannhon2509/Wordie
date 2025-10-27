namespace Wordie.Server.Domain.Entities;

public class WordTag
{
    public int WordCardId { get; set; }
    public int TagId { get; set; }

    // Navigation
    public WordCard WordCard { get; set; } = default!;
    public Tag Tag { get; set; } = default!;
}
