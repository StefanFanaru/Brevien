using System;
using Ardalis.GuardClauses;
using Posting.Core.Common;

namespace Posting.Core.Entities
{
    public class Comment : Entity<string>
    {
        public Comment(string userId, string postId, string content) : base(Guid.NewGuid().ToString())
        {
            Guard.Against.NullOrEmpty(userId, nameof(userId));
            Guard.Against.NullOrEmpty(postId, nameof(postId));
            Guard.Against.NullOrEmpty(content, nameof(content));

            UserId = userId;
            PostId = postId;
            Content = content;
            CreatedAt = DateTime.UtcNow;
        }

        public string UserId { get; set; }
        public string PostId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public void Edit(string content)
        {
            if (Content != content)
            {
                Guard.Against.NullOrEmpty(content, nameof(content));
                Content = content;
                UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}