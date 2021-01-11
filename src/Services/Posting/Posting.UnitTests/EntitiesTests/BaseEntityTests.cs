using System;
using FluentAssertions;
using Posting.Core.Entities;
using Xunit;

namespace Posting.UnitTests.EntitiesTests
{
    public class BaseEntityTests
    {
        [Fact]
        public void Cannot_use_default_value_int_id()
        {
            BaseEntityTestClassIntId entityIntId = null;
            ArgumentException exception = null;

            try
            {
                entityIntId = new BaseEntityTestClassIntId(0);
            }
            catch (ArgumentException e)
            {
                exception = e;
            }

            entityIntId.Should().BeNull();
            exception.Should().NotBeNull();
        }

        [Fact]
        public void Cannot_use_default_value_string_id()
        {
            BaseEntityTestClassStringId entityStringId = null;
            ArgumentException exception = null;

            try
            {
                entityStringId = new BaseEntityTestClassStringId(null);
            }
            catch (ArgumentException e)
            {
                exception = e;
            }

            entityStringId.Should().BeNull();
            exception.Should().NotBeNull();
        }
    }

    public class BaseEntityTestClassStringId : Entity<string>
    {
        public BaseEntityTestClassStringId(string id) : base(id)
        {
        }
    }

    public class BaseEntityTestClassIntId : Entity<int>
    {
        public BaseEntityTestClassIntId(int id) : base(id)
        {
        }
    }
}