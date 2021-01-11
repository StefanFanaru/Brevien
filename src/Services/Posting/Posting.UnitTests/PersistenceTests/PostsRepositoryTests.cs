using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Posting.Core.Entities;
using Posting.Infrastructure.Data.Configuration;
using Posting.Infrastructure.Data.Repositories;
using Xunit;

namespace Posting.UnitTests.PersistenceTests
{
    [Collection("Sequential")]
    public class PostsRepositoryTests
    {
        private readonly CommentsRepository _commentsRepository;
        private readonly DapperRepository<Post> _postRepository;

        public PostsRepositoryTests()
        {
            var connectionProvider = SqliteConnectionProvider.Instance;
            connectionProvider.Reset();
            var connection = connectionProvider.GetConnection();
            connection.CreateTables(true);
            _postRepository = new DapperRepository<Post>(connectionProvider);
            _commentsRepository = new CommentsRepository(connectionProvider);
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

            await _postRepository.InsertAsync(post);
            await _commentsRepository.InsertBatchAsync(expected);

            var actual = await _commentsRepository.GetByPostId(post.Id);
            expected.Should().BeEquivalentTo(actual);
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

            await _postRepository.InsertAsync(post);
            await _commentsRepository.InsertBatchAsync(expected);

            var actual = await _commentsRepository.GetByUserId(TestConstants.UserId);
            expected.Should().BeEquivalentTo(actual);
        }
    }
}