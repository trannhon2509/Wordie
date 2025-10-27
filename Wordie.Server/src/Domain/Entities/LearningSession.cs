namespace Wordie.Server.Domain.Entities;

public class LearningSession : BaseAuditableEntity
{
    // Use string user id to avoid domain dependency on ApplicationUser
    public string UserId { get; set; } = default!;
    public int WordSetId { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }

    public int WordsStudied { get; set; }
    public int CorrectAnswers { get; set; }
    public int IncorrectAnswers { get; set; }

    // Navigation
    // No User navigation here; map UserId to ApplicationUser in Infrastructure if needed
    public WordSet WordSet { get; set; } = default!;
}
