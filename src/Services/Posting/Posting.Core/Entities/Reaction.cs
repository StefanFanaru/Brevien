using System;
using Ardalis.GuardClauses;
using Posting.Core.Common;
using Posting.Core.Enums;

namespace Posting.Core.Entities
{
    public class Reaction : Entity<string>
    {
        public Reaction(string userId, string postId, ReactionType type) : base(Guid.NewGuid().ToString())
        {
            Guard.Against.NullOrWhiteSpace(userId, nameof(userId));
            Guard.Against.NullOrWhiteSpace(postId, nameof(postId));

            UserId = userId;
            PostId = postId;
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }

        public string UserId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}