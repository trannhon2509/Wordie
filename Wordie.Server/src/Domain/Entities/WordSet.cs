namespace Wordie.Server.Domain.Entities;

public class WordSet : BaseAuditableEntity
{
    // Keep integer Id to match project BaseEntity design
    // (If you prefer Guid keys across the project we can migrate BaseEntity later)
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public bool IsSystem { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    // Creator information is stored via BaseAuditableEntity.CreatedBy (string) or CreatorId (string) in infrastructure; keep Domain independent.
    public string? CreatorId { get; set; }
    public ICollection<WordCard> WordCards { get; set; } = new List<WordCard>();
}
