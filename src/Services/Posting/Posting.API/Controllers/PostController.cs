using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Posting.API.Helpers;
using Posting.Infrastructure.Commands;

namespace Posting.API.Controllers
{
    [Authorize(Policy = "BlogOwner")]
    [Route("api/v1/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostCommand command)
        {
            return this.Result(await _mediator.Send(command, HttpContext.RequestAborted));
        }
    }
}
