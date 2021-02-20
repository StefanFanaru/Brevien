using System;
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
    public class DapperRepositoryTests
    {
        private readonly DapperRepository _repository;

        public DapperRepositoryTests()
        {
            var connectionProvider = TestHelpers.InitializeInMemoryDb();
            _repository = new DapperRepository(connectionProvider);
        }

        [Fact]
        public async Task Records_can_be_inserted()
        {
            var expected = Builders.GetPostEntity();
            await _repository.InsertAsync(expected);
            var actual = await _repository.GetAsync<Post>(expected.Id);

            actual.Should().BeEquivalentTo(expected);
        }


        [Fact]
        public async Task GetAll_returns_all_records()
        {
            // Arrange
            var records = new List<Post>();
            for (var i = 0; i < 5; i++)
            {
                var record = Builders.GetPostEntity();
                records.Add(record);
                await _repository.InsertAsync(record);
            }

            // Act
            var posts = await _repository.GetAllAsync<Post>();

            // Assert
            var result = posts.ToList();
            result.Count.Should().Be(5);
            result.Should().BeEquivalentTo(records);
        }

        [Fact]
        public async Task Record_is_deleted()
        {
            var record = Builders.GetPostEntity();
            await _repository.InsertAsync(record);

            await _repository.DeleteAsync<Post>(record.Id);
            var post = await _repository.GetAsync<Post>(record.Id);

            post.Should().BeNull();
        }

        [Fact]
        public async Task Record_can_be_inserted_and_retrieved()
        {
            var record = Builders.GetPostEntity();
            await _repository.InsertAsync(record);

            var post = await _repository.GetAsync<Post>(record.Id);

            post.Should().BeEquivalentTo(record);
        }

        [Fact]
        public async Task Record_can_be_updated()
        {
            var record = Builders.GetPostEntity();
            var newTitle = "New test title";
            await _repository.InsertAsync(record);

            record.EditTitle(newTitle);
            await _repository.UpdateAsync(record);
            var post = await _repository.GetAsync<Post>(record.Id);

            post.Title.Should().Be(newTitle);
            post.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, 100);
        }

        [Fact]
        public async Task Multiple_records_can_be_inserted_simultaneously()
        {
            var expected = new List<Post> {Builders.GetPostEntity(), Builders.GetPostEntity(), Builders.GetPostEntity()};
            await _repository.InsertBatchAsync(expected);
            var actual = await _repository.GetAllAsync<Post>();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}