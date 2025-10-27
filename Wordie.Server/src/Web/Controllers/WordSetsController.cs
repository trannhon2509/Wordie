using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Wordie.Server.Application.WordSets.Commands.CreateWordSet;
using Wordie.Server.Application.WordSets.Commands.UpdateWordSet;
using Wordie.Server.Application.WordSets.Commands.DeleteWordSet;
using Wordie.Server.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Wordie.Server.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WordSetsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUser _currentUser;
    private readonly IApplicationDbContext _context;

    public WordSetsController(IMediator mediator, IUser currentUser, IApplicationDbContext context)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _context = context;
    }

    /// <summary>
    /// Get all word sets.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var items = await _mediator.Send(new Wordie.Server.Application.WordSets.Queries.GetWordSets.GetWordSetsQuery());
        return Ok(items);
    }

    /// <summary>
    /// Create a new word set (authenticated users only).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateWordSetCommand request)
    {
        var id = await _mediator.Send(request);
        return Created($"/api/WordSets/{id}", new { id });
    }

    /// <summary>
    /// Update a word set (authenticated users only).
    /// </summary>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] UpdateWordSetCommand request)
    {
        await _mediator.Send(request);
        return NoContent();
    }

    /// <summary>
    /// Delete a word set. Requires Admin role.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteWordSetCommand(id));
        return NoContent();
    }
}
