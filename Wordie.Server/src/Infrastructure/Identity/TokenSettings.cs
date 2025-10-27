namespace Wordie.Server.Infrastructure.Identity;

public class TokenSettings
{
    public string Issuer { get; set; } = "Wordie";
    public string Audience { get; set; } = "WordieClients";
    public string SigningKey { get; set; } = "ReplaceThisWithAStrongKeyInProduction";
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
