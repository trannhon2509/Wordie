using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Wordie.Server.Infrastructure.Identity;
using Wordie.Server.Application.Common.Interfaces;

namespace Wordie.Server.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly Wordie.Server.Application.Common.Interfaces.ITokenService _tokenService;

    public UsersController(IIdentityService identityService, UserManager<ApplicationUser> userManager, Wordie.Server.Application.Common.Interfaces.ITokenService tokenService)
    {
        _identityService = identityService;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public record RegisterRequest(string Email, string Password);
    public record LoginRequest(string Email, string Password);
    public record RefreshRequest(string RefreshToken);

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var (result, userId) = await _identityService.CreateUserAsync(request.Email, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Created($"/api/Users/{userId}", new { userId });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email) ?? await _userManager.FindByNameAsync(request.Email);
        if (user == null) return Unauthorized();

        var valid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!valid) return Unauthorized();

            var tokens = await _tokenService.GenerateTokensAsync(user.Id, user.UserName ?? string.Empty);

        return Ok(new
        {
            accessToken = tokens.AccessToken,
            accessTokenExpires = tokens.AccessTokenExpires,
            refreshToken = tokens.RefreshToken,
            refreshTokenExpires = tokens.RefreshTokenExpires,
        });
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken)) return BadRequest();

        var result = await _tokenService.RefreshAsync(request.RefreshToken);
        if (result == null) return Unauthorized();

        return Ok(new
        {
            accessToken = result.AccessToken,
            accessTokenExpires = result.AccessTokenExpires,
            refreshToken = result.RefreshToken,
            refreshTokenExpires = result.RefreshTokenExpires,
        });
    }
}
