﻿using MercuryBus.Events.Common;

namespace Blogging.API.Events
{
    public class BlogCreatedEvent : IDomainEvent
    {
        public string BlogId { get; set; }
        public string UserId { get; set; }
    }
}