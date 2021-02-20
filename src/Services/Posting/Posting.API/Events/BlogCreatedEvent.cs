using MercuryBus.Events.Common;

namespace Posting.API.Events
{
    public class BlogCreatedEvent : IDomainEvent
    {
        public string BlogId { get; set; }
        public string UserId { get; set; }
        public string BlogName { get; set; }
        public string BlogUri { get; set; }
    }
}