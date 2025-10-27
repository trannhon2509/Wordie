using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Application.Common.Models;
using Wordie.Server.Infrastructure.Data;

namespace Wordie.Server.Infrastructure.Identity;

public class TokenService : ITokenService
{
    private readonly TokenSettings _settings;
    private readonly ApplicationDbContext _dbContext;
    private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager; // Keep this for internal use

    

    public TokenService(IOptions<TokenSettings> settings, ApplicationDbContext dbContext, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
    {
        _settings = settings.Value;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<TokenResult> GenerateTokensAsync(string userId, string userName)
    {
        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, userName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // include user roles if any
        var user = await _userManager.FindByIdAsync(userId);
        var roles = user != null ? await _userManager.GetRolesAsync(user) : new List<string>();
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var accessExpires = now.AddMinutes(_settings.AccessTokenExpirationMinutes);

        var jwt = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: accessExpires,
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        // create refresh token (generate a strong random token, store only its SHA256 hash)
        var rawRefreshBytes = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(rawRefreshBytes);
        var rawRefreshToken = Convert.ToBase64String(rawRefreshBytes);

        // compute SHA256 hash of the raw token for storage
        string ComputeHash(string input)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hashed = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hashed);
        }

        var refresh = new RefreshToken
        {
            TokenHash = ComputeHash(rawRefreshToken),
            UserId = userId,
            Created = now,
            Expires = now.AddDays(_settings.RefreshTokenExpirationDays)
        };

        _dbContext.RefreshTokens.Add(refresh);
        await _dbContext.SaveChangesAsync();

        return new TokenResult(accessToken, rawRefreshToken, accessExpires, refresh.Expires);
    }

    public async Task<TokenResult?> RefreshAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken)) return null;

        // compute hash of provided refresh token and look up
        using var sha = System.Security.Cryptography.SHA256.Create();
        var rtBytes = System.Text.Encoding.UTF8.GetBytes(refreshToken);
        var rtHash = Convert.ToBase64String(sha.ComputeHash(rtBytes));

        var stored = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == rtHash);
        if (stored == null || stored.IsExpired) return null;

        var user = await _userManager.FindByIdAsync(stored.UserId);
        if (user == null) return null;

        // remove the used refresh token to prevent reuse
        _dbContext.RefreshTokens.Remove(stored);
        await _dbContext.SaveChangesAsync();

        return await GenerateTokensAsync(user.Id, user.UserName ?? string.Empty);
    }
}
