using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Wordie.Server.Application.UserWordProgress.Commands.CreateUserWordProgress;
using Wordie.Server.Application.UserWordProgress.Commands.UpdateUserWordProgress;
using Wordie.Server.Application.UserWordProgress.Commands.DeleteUserWordProgress;
using Wordie.Server.Application.UserWordProgress.Queries.GetUserWordProgress;
using Wordie.Server.Application.UserWordProgress.Queries.GetUserWordProgressList;

namespace Wordie.Server.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserWordProgressController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly Wordie.Server.Application.Common.Interfaces.IUser _currentUser;

    public UserWordProgressController(IMediator mediator, Wordie.Server.Application.Common.Interfaces.IUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Get progress of the authenticated user for a word card.
    /// </summary>
    [HttpGet("{wordCardId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(int wordCardId)
    {
        var userId = _currentUser.Id;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var dto = await _mediator.Send(new GetUserWordProgressQuery(userId, wordCardId));
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    /// <summary>
    /// Get all progress entries for the authenticated user.
    /// </summary>
    [HttpGet("user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetForUser()
    {
        var userId = _currentUser.Id;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var list = await _mediator.Send(new GetUserWordProgressListQuery(userId));
        return Ok(list);
    }

    public record CreateRequest(int WordCardId, int Level, DateTime NextReviewAt);
    /// <summary>
    /// Create a new user word progress for the authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateRequest request)
    {
        var userId = _currentUser.Id;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        await _mediator.Send(new CreateUserWordProgressCommand
        {
            UserId = userId,
            WordCardId = request.WordCardId,
            Level = request.Level,
            NextReviewAt = request.NextReviewAt
        });

        return Accepted();
    }

    public record UpdateRequest(int WordCardId, int Level, DateTime NextReviewAt, int CorrectCount, int IncorrectCount);
    /// <summary>
    /// Update progress for a word card for the authenticated user.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] UpdateRequest request)
    {
        var userId = _currentUser.Id;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        await _mediator.Send(new UpdateUserWordProgressCommand
        {
            UserId = userId,
            WordCardId = request.WordCardId,
            Level = request.Level,
            NextReviewAt = request.NextReviewAt,
            CorrectCount = request.CorrectCount,
            IncorrectCount = request.IncorrectCount
        });

        return NoContent();
    }

    /// <summary>
    /// Delete the progress entry for the authenticated user and a word card.
    /// </summary>
    [HttpDelete("{wordCardId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int wordCardId)
    {
        var userId = _currentUser.Id;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        await _mediator.Send(new DeleteUserWordProgressCommand(userId, wordCardId));
        return NoContent();
    }
}
