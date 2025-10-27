using Microsoft.AspNetCore.Mvc;
using MediatR;
using Wordie.Server.Application.Tags.Commands.CreateTag;
using Wordie.Server.Application.Tags.Commands.DeleteTag;
using Wordie.Server.Application.Tags.Queries.GetTags;

namespace Wordie.Server.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vm = await _mediator.Send(new GetTagsQuery());
        return Ok(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagCommand request)
    {
        var id = await _mediator.Send(request);
        return Created($"/api/Tags/{id}", new { id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteTagCommand(id));
        return NoContent();
    }
}
