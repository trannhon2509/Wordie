using Microsoft.AspNetCore.Mvc;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Wordie.Server.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize]
public class WordSynonymsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUser _currentUser;

    public WordSynonymsController(IMediator mediator, IUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    public record CreateRequest(int WordCardId, int SynonymCardId);

    /// <summary>
    /// Create a synonym link between two word cards.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateRequest request)
    {
        var creatorId = _currentUser.Id ?? string.Empty;
        await _mediator.Send(new Wordie.Server.Application.WordSynonyms.Commands.CreateWordSynonym.CreateWordSynonymCommand(request.WordCardId, request.SynonymCardId, creatorId));
        return Accepted();
    }

    /// <summary>
    /// Delete a synonym link between two word cards.
    /// </summary>
    [HttpDelete("{wordCardId}/{synonymCardId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int wordCardId, int synonymCardId)
    {
        await _mediator.Send(new Wordie.Server.Application.WordSynonyms.Commands.DeleteWordSynonym.DeleteWordSynonymCommand(wordCardId, synonymCardId));
        return NoContent();
    }
}
