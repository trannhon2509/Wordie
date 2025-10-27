using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Wordie.Server.Application.WordCards.Commands.CreateWordCard;
using Wordie.Server.Application.WordCards.Commands.UpdateWordCard;
using Wordie.Server.Application.WordCards.Commands.DeleteWordCard;
using Wordie.Server.Application.WordCards.Queries.GetWordCard;
using Wordie.Server.Application.Common.Interfaces;

namespace Wordie.Server.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WordCardsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUser _currentUser;

    public WordCardsController(IMediator mediator, IUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Get a word card by id.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var dto = await _mediator.Send(new GetWordCardQuery(id));
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    /// <summary>
    /// Create a new word card (authenticated users only).
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateWordCardCommand request)
    {
        var id = await _mediator.Send(request);
        return Created($"/api/WordCards/{id}", new { id });
    }

    /// <summary>
    /// Update an existing word card (authenticated users only).
    /// </summary>
    [HttpPut]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] UpdateWordCardCommand request)
    {
        await _mediator.Send(request);
        return NoContent();
    }

    /// <summary>
    /// Delete a word card. Requires Admin role.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteWordCardCommand(id));
        return NoContent();
    }
}
