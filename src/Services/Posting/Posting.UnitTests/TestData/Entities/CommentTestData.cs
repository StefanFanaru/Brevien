using System.Collections;
using System.Collections.Generic;

namespace Posting.UnitTests.TestData.Entities
{
    public class CommentTestData
    {
        public class CommentConstructorParameters : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] {"test", "test", null};
                yield return new object[] {"test", null, "test"};
                yield return new object[] {null, "test", "test"};
                yield return new object[] {"test", "test", ""};
                yield return new object[] {"test", "", "test"};
                yield return new object[] {"", "test", "test"};
                yield return new object[] {"test", "test", " "};
                yield return new object[] {"test", " ", "test"};
                yield return new object[] {" ", "test", "test"};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}