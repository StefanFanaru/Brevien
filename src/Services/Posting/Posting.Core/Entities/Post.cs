using System;
using Ardalis.GuardClauses;
using Posting.Core.Common;

namespace Posting.Core.Entities
{
    public class Post : Entity<string>
    {
        public Post()
        {
        }

        public Post(string title, string content, string url, string userId, string blogId) : base(Guid.NewGuid().ToString())
        {
            Guard.Against.NullOrEmpty(title, nameof(title));
            Guard.Against.NullOrEmpty(content, nameof(content));
            Guard.Against.NullOrEmpty(url, nameof(url));
            Guard.Against.NullOrEmpty(userId, nameof(userId));
            Guard.Against.NullOrEmpty(blogId, nameof(blogId));

            Title = title;
            Content = content;
            Url = url;
            UserId = userId;
            BlogId = blogId;
            CreatedAt = DateTime.UtcNow;
        }

        public string Title { get; private set; }
        public string Content { get; private set; }
        public string Url { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string UserId { get; private set; }
        public string BlogId { get; private set; }

        public void EditContent(string content)
        {
            Guard.Against.NullOrEmpty(content, nameof(content));
            Content = content;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeUrl(string url)
        {
            Guard.Against.NullOrEmpty(url, nameof(url));
            Url = url;
            UpdatedAt = DateTime.UtcNow;
        }

        public void EditTitle(string title)
        {
            Guard.Against.NullOrEmpty(title, nameof(title));
            Title = title;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}