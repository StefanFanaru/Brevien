using System.Collections;
using System.Collections.Generic;
using Posting.Core.Enums;

namespace Posting.UnitTests.TestData.Entities
{
    public class ReactionTestData
    {
        public class ReactionConstructorParameters : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] {"test", null, ReactionType.Like};
                yield return new object[] {null, "test", ReactionType.Like};
                yield return new object[] {"test", "", ReactionType.Like};
                yield return new object[] {"", "test", ReactionType.Like};
                yield return new object[] {"test", " ", ReactionType.Like};
                yield return new object[] {" ", "test", ReactionType.Like};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}