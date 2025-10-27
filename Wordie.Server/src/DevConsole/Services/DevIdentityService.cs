using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Application.Common.Models;

namespace DevConsole.Services;

/// <summary>
/// Simple in-memory identity service for the DevConsole.
/// Stores users and roles in memory and supports minimal operations used by the application.
/// </summary>
public class DevIdentityService : IIdentityService
{
    private readonly Dictionary<string, (string Password, HashSet<string> Roles)> _users = new();

    public Task<string?> GetUserNameAsync(string userId)
    {
        if (_users.TryGetValue(userId, out var entry))
            return Task.FromResult<string?>(userId);

        return Task.FromResult<string?>(null);
    }

    public Task<bool> IsInRoleAsync(string userId, string role)
    {
        if (_users.TryGetValue(userId, out var entry))
            return Task.FromResult(entry.Roles.Contains(role));

        return Task.FromResult(false);
    }

    public Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        // Simple policies: 'AdminOnly' -> Admin role present
        if (policyName == "AdminOnly")
            return Task.FromResult(_users.TryGetValue(userId, out var e) && e.Roles.Contains("Admin"));

        return Task.FromResult(true);
    }

    public Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Task.FromResult((Result.Failure(new[] { "Invalid user name" }), string.Empty));

        if (_users.ContainsKey(userName))
            return Task.FromResult((Result.Failure(new[] { "User already exists" }), userName));

        _users[userName] = (password ?? string.Empty, new HashSet<string> { "User" });

        return Task.FromResult((Result.Success(), userName));
    }

    public Task<Result> DeleteUserAsync(string userId)
    {
        _users.Remove(userId);
        return Task.FromResult(Result.Success());
    }

    // Dev helper: allow adding a role to a user
    public void AddRole(string userId, string role)
    {
        if (!_users.TryGetValue(userId, out var entry)) return;
        entry.Roles.Add(role);
        _users[userId] = entry;
    }
}

