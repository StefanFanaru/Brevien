using System.Collections;
using System.Collections.Generic;

namespace Posting.UnitTests.TestData.Entities
{
    public class PostTestData
    {
        public class PostConstructorParameters : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] {"test", "test", "test", "test", null};
                yield return new object[] {"test", "test", "test", null, "test"};
                yield return new object[] {"test", "test", null, "test", "test"};
                yield return new object[] {"test", null, "test", "test", "test"};
                yield return new object[] {null, "test", "test", "test", "test"};
                yield return new object[] {"test", "test", "test", "test", ""};
                yield return new object[] {"test", "test", "test", "", "test"};
                yield return new object[] {"test", "test", "", "test", "test"};
                yield return new object[] {"test", "", "test", "test", "test"};
                yield return new object[] {"", "test", "test", "test", "test"};
                yield return new object[] {"test", "test", "test", "test", " "};
                yield return new object[] {"test", "test", "test", " ", "test"};
                yield return new object[] {"test", "test", " ", "test", "test"};
                yield return new object[] {"test", " ", "test", "test", "test"};
                yield return new object[] {" ", "test", "test", "test", "test"};
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}