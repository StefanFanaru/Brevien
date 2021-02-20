using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Posting.Core.Entities;
using Posting.Infrastructure.Data.Repositories;
using Xunit;

namespace Posting.UnitTests.PersistenceTests
{
    [Collection("Sequential")]
    public class PostsRepositoryTests
    {
        private readonly DapperRepository _repository;

        public PostsRepositoryTests()
        {
            var connectionProvider = TestHelpers.InitializeInMemoryDb();
            _repository = new DapperRepository(connectionProvider);
        }

        [Fact]
        public async Task Can_retrieve_all_comments_of_a_post()
        {
            var post = Builders.GetPostEntity();
            var expected = new List<Comment>
            {
                Builders.GetCommentEntity(post.Id),
                Builders.GetCommentEntity(post.Id),
                Builders.GetCommentEntity(post.Id)
            };

            await _repository.InsertAsync(post);
            await _repository.InsertBatchAsync(expected);

            var actual = await _repository.GetByKeyAsync<Comment>("PostId", post.Id);
            actual.Count().Should().Be(expected.Count);
        }

        [Fact]
        public async Task Can_retrieve_all_comments_of_a_user()
        {
            var post = Builders.GetPostEntity();
            var expected = new List<Comment>
            {
                Builders.GetCommentEntity(post.Id),
                Builders.GetCommentEntity(post.Id),
                Builders.GetCommentEntity(post.Id)
            };

            await _repository.InsertAsync(post);
            await _repository.InsertBatchAsync(expected);

            var actual = await _repository.GetByKeyAsync<Comment>(nameof(post.UserId), TestConstants.UserId);
            actual.Count().Should().Be(expected.Count);
        }
    }
}