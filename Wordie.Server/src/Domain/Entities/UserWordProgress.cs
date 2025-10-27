namespace Wordie.Server.Domain.Entities;

using Wordie.Server.Domain.Common;

public class UserWordProgress : BaseEntity
{
    // Store user id as string to avoid referencing domain User/ApplicationUser
    public string UserId { get; set; } = default!;
    public int WordCardId { get; set; }

    public int Level { get; set; }
    public DateTime LastReviewedAt { get; set; } = DateTime.UtcNow;
    public DateTime NextReviewAt { get; set; }
    public int CorrectCount { get; set; }
    public int IncorrectCount { get; set; }

    // Navigation
    // No User navigation in Domain layer; Infrastructure may map UserId to ApplicationUser
    public WordCard WordCard { get; set; } = default!;
}
