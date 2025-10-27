using Wordie.Server.Application.Common.Interfaces;

namespace DevConsole.Services;

public class DevUser : IUser
{
    // For the dev console, allow setting the current user via environment variable DEV_USER_ID or default to 'dev'
    private readonly string? _id = Environment.GetEnvironmentVariable("DEV_USER_ID") ?? "dev";
    private readonly List<string> _roles = new List<string> { "User" };

    public DevUser()
    {
        // If a DEV_USER_ROLES env var is present, parse it as comma-separated roles
        var roles = Environment.GetEnvironmentVariable("DEV_USER_ROLES");
        if (!string.IsNullOrWhiteSpace(roles))
        {
            _roles.Clear();
            _roles.AddRange(roles.Split(',').Select(r => r.Trim()).Where(r => !string.IsNullOrEmpty(r)));
        }
    }

    public string? Id => _id;

    public List<string>? Roles => _roles;
}
