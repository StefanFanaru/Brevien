using MercuryBus.Events.Subscriber;
using Posting.Core.Entities.External;
using Posting.Core.Interfaces.Data;
using Posting.Infrastructure.Helpers;

namespace Posting.API.Events
{
    public class BlogCreatedEventHandler : IDomainEventHandler<BlogCreatedEvent>
    {
        private readonly IDapperRepository _repository;

        public BlogCreatedEventHandler(IDapperRepository repository)
        {
            _repository = repository;
        }

        public void Handle(IDomainEventEnvelope<BlogCreatedEvent> @event)
        {
            var payload = @event.Message.Payload.FromJson<BlogCreatedEvent>();


            if (_repository.Any<BlogOwner>(("UserId", payload.UserId, null), ("BlogId", payload.BlogId, "AND")))
            {
                return;
            }

            _repository.Insert(new Blog
            {
                Id = payload.BlogId,
                Name = payload.BlogName,
                Uri = payload.BlogUri
            });

            _repository.Insert(new BlogOwner
            {
                BlogId = payload.BlogId,
                UserId = payload.UserId
            });
        }
    }
}