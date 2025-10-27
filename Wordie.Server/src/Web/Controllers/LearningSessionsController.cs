using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Wordie.Server.Application.LearningSessions.Commands.CreateLearningSession;
using Wordie.Server.Application.LearningSessions.Commands.UpdateLearningSession;
using Wordie.Server.Application.LearningSessions.Commands.DeleteLearningSession;
using Wordie.Server.Application.LearningSessions.Queries.GetLearningSession;
using Wordie.Server.Application.LearningSessions.Queries.GetLearningSessionList;

namespace Wordie.Server.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LearningSessionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly Wordie.Server.Application.Common.Interfaces.IUser _currentUser;

    public LearningSessionsController(IMediator mediator, Wordie.Server.Application.Common.Interfaces.IUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Get a learning session by id for the authenticated user.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(int id)
    {
        var dto = await _mediator.Send(new GetLearningSessionQuery(id));
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    /// <summary>
    /// Get learning sessions for the authenticated user.
    /// </summary>
    [HttpGet("user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetForUser()
    {
        var userId = _currentUser.Id;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var list = await _mediator.Send(new GetLearningSessionListQuery(userId));
        return Ok(list);
    }

    public record CreateRequest(int WordSetId);
    /// <summary>
    /// Create a learning session for the authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateRequest request)
    {
        var userId = _currentUser.Id;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var id = await _mediator.Send(new CreateLearningSessionCommand { UserId = userId, WordSetId = request.WordSetId });
        return Created($"/api/LearningSessions/{id}", new { id });
    }

    public record UpdateRequest(int Id, DateTime? EndedAt, int WordsStudied, int CorrectAnswers, int IncorrectAnswers);
    /// <summary>
    /// Update a learning session.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] UpdateRequest request)
    {
        await _mediator.Send(new UpdateLearningSessionCommand
        {
            Id = request.Id,
            EndedAt = request.EndedAt,
            WordsStudied = request.WordsStudied,
            CorrectAnswers = request.CorrectAnswers,
            IncorrectAnswers = request.IncorrectAnswers
        });

        return NoContent();
    }

    /// <summary>
    /// Delete a learning session.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteLearningSessionCommand(id));
        return NoContent();
    }
}
