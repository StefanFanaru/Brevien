using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Posting.Core.Entities;
using Posting.Core.Interfaces.Asp;
using Posting.Core.Interfaces.Data;
using Posting.Infrastructure.Operations;

namespace Posting.Infrastructure.Commands
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, IOperationResult<Unit>>
    {
        private readonly IRepository<Post> _repository;
        private readonly IUserInfo _userInfo;

        public CreatePostCommandHandler(IRepository<Post> repository, IUserInfo userInfo)
        {
            _repository = repository;
            _userInfo = userInfo;
        }

        public async Task<IOperationResult<Unit>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            await _repository.InsertAsync(new Post(request.Title, request.Content, request.Url, _userInfo.Id, request.BlogId));
            return ResultBuilder.Ok();
        }
    }
}