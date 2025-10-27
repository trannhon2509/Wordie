namespace Wordie.Server.Application.Common.Models;

public record TokenResult(string AccessToken, string RefreshToken, System.DateTime AccessTokenExpires, System.DateTime RefreshTokenExpires);
