using Posting.Core.Entities;

namespace Posting.UnitTests
{
    public static class Builders
    {
        public static Post GetPostEntity()
        {
            return new Post(
                "Test post",
                "This is a simple test post, written for tests",
                "https://test.post/test-post", "591a4046-ee26-42e4-a836-970b23fbb47e",
                "0db4fd74-ca72-4f69-9bb8-5756e08d65ff");
        }
    }
}