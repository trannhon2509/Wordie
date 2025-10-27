using System.ComponentModel.DataAnnotations;

namespace Wordie.Server.Infrastructure.Identity;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    // Store only a hash of the token (SHA256) to avoid keeping raw tokens in the DB
    public string TokenHash { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public DateTime Expires { get; set; }

    public DateTime Created { get; set; }

    public bool IsExpired => DateTime.UtcNow >= Expires;
}

