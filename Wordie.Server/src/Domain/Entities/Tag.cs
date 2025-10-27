namespace Wordie.Server.Domain.Entities;

public class Tag : BaseAuditableEntity
{
    public string Name { get; set; } = default!;

    public bool IsSystem { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    // Store creator id as string (no direct domain dependency on ApplicationUser)
    public string? CreatorId { get; set; }
    public ICollection<WordTag> WordTags { get; set; } = new List<WordTag>();
}
