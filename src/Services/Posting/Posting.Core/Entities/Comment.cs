using System;
using Ardalis.GuardClauses;

namespace Posting.Core.Entities
{
    public class Comment : Entity<string>
    {
        public Comment(string userId, string postId, string content) : base(Guid.NewGuid().ToString())
        {
            Guard.Against.NullOrWhiteSpace(userId, nameof(userId));
            Guard.Against.NullOrWhiteSpace(postId, nameof(postId));
            Guard.Against.NullOrWhiteSpace(content, nameof(content));

            UserId = userId;
            PostId = postId;
            Content = content;
            CreatedAt = DateTime.UtcNow;
        }

        public Comment()
        {
        }

        public string UserId { get; private set; }
        public string PostId { get; private set; }
        public string Content { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public void EditContent(string content)
        {
            Guard.Against.NullOrWhiteSpace(content, nameof(content));
            Content = content;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}