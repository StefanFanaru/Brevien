using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Posting.Core.Entities;
using Posting.Core.Interfaces.Asp;
using Posting.Core.Interfaces.Data;
using Posting.Infrastructure.Operations;

namespace Posting.Infrastructure.Commands
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, IOperationResult<string>>
    {
        private readonly IDapperRepository _repository;
        private readonly IUserInfo _userInfo;

        public CreatePostCommandHandler(IDapperRepository repository, IUserInfo userInfo)
        {
            _repository = repository;
            _userInfo = userInfo;
        }

        public async Task<IOperationResult<string>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var entity = new Post(request.Title, request.Content, request.Url, _userInfo.Id, request.BlogId);
            await _repository.InsertAsync(entity);
            return ResultBuilder.Created<string>(entity.Id);
        }
    }
}