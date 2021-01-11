using Posting.Core.Entities;
using Posting.Core.Enums;

namespace Posting.UnitTests
{
    public static class Builders
    {
        public static Post GetPostEntity()
        {
            return new(
                "Test post",
                "This is a simple test post, written for tests",
                "https://test.post/test-post",
                TestConstants.UserId,
                "0db4fd74-ca72-4f69-9bb8-5756e08d65ff");
        }

        public static Comment GetCommentEntity(string postId = "5dd82d10-3245-4bb9-9b33-0a9228dc4be6")
        {
            return new(
                TestConstants.UserId,
                postId,
                "This is a simple test comment, written for tests");
        }

        public static Reaction GetReactionEntity(string postId = "09f21c41-1f26-49e2-83c4-1568d008ed19")
        {
            return new(
                TestConstants.UserId,
                postId,
                ReactionType.Like);
        }
    }
}