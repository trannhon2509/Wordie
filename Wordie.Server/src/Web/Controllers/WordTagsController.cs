using Microsoft.AspNetCore.Mvc;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize]
public class WordTagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WordTagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public record CreateRequest(int WordCardId, int TagId);

    /// <summary>
    /// Create a tag association for a word card.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateRequest request)
    {
        await _mediator.Send(new Wordie.Server.Application.WordTags.Commands.CreateWordTag.CreateWordTagCommand(request.WordCardId, request.TagId));
        return Accepted();
    }

    /// <summary>
    /// Delete a tag association from a word card.
    /// </summary>
    [HttpDelete("{wordCardId}/{tagId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int wordCardId, int tagId)
    {
        await _mediator.Send(new Wordie.Server.Application.WordTags.Commands.DeleteWordTag.DeleteWordTagCommand(wordCardId, tagId));
        return NoContent();
    }
}
