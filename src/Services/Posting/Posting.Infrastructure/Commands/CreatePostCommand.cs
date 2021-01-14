using MediatR;
using Posting.Core.Interfaces.Asp;

namespace Posting.Infrastructure.Commands
{
    public class CreatePostCommand : IRequest<IOperationResult<string>>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string BlogId { get; set; }
        public string Url { get; set; }
    }
}