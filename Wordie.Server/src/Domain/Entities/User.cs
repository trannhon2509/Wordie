namespace Wordie.Server.Domain.Entities;

public class User : BaseAuditableEntity
{
    // Id comes from BaseEntity (int)
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<WordSet> WordSets { get; set; } = new List<WordSet>();
    public ICollection<UserWordProgress> WordProgresses { get; set; } = new List<UserWordProgress>();
}
