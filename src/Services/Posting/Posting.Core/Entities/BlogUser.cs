namespace Posting.Core.Entities
{
    public class BlogUser : Entity<string>
    {
        public string BlogId { get; set; }
        public string UserId { get; set; }
    }
}