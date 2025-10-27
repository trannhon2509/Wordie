using Wordie.Server.Application.Common.Models;

namespace Wordie.Server.Application.Common.Interfaces;

public interface ITokenService
{
    Task<TokenResult> GenerateTokensAsync(string userId, string userName);

    Task<TokenResult?> RefreshAsync(string refreshToken);
}
