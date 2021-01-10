using System;
using FluentAssertions;
using Posting.Core.Entities;
using Posting.Core.Enums;
using Posting.UnitTests.TestData.Entities;
using Xunit;

namespace Posting.UnitTests.EntitiesTests
{
    public class ReactionTests
    {
        [Theory]
        [ClassData(typeof(ReactionTestData.ReactionConstructorParameters))]
        public void Cannot_instantiate_with_null_or_whitespace_parameter(string userId, string postId, ReactionType type)
        {
            Reaction reaction = null;
            ArgumentException exception = null;
            try
            {
                reaction = new Reaction(userId, postId, type);
            }
            catch (ArgumentException e)
            {
                exception = e;
            }

            reaction.Should().BeNull();
            exception.Should().NotBeNull();
        }

        [Fact]
        public void Can_instantiate_reaction()
        {
            var reaction = new Reaction(
                "c283e120-e6cf-4ea4-9adc-74c36d436a7d",
                "09f21c41-1f26-49e2-83c4-1568d008ed19",
                ReactionType.Like);

            reaction.Should().NotBeNull();
            reaction.Id.Should().NotBeNullOrWhiteSpace();
            reaction.Type.Should().NotBeNull();
            reaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 100);
            reaction.PostId.Should().NotBeNullOrWhiteSpace();
            reaction.UserId.Should().NotBeNullOrWhiteSpace();
        }
    }
}