using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Posting.Core.Entities;

namespace Posting.UnitTests.TestData.Entities
{
    public class PostTestData
    {
        public class PostConstructorParameters : IEnumerable<object[]>
        {
            // Constructor parameters 
            // 0. title
            // 1. content
            // 2. url
            // 3. userId
            // 4. blogId

            // If constructor parameters order changes or new ones are added just change the indexes bellow and update the list above
            // I guess this is overkill but it's just for the sake of experimenting and learning.

            private readonly string[] _constructorParameters = typeof(Post).GetConstructors()
                .Where(c => c.GetParameters().Any())
                .Select(x => x.GetParameters())
                .SelectMany(x => x.Select(p => p.Name))
                .ToArray();

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                    {"test title", "test content", "test url", "test userId", null, _constructorParameters[4]};
                yield return new object[]
                    {"test title", "test content", "test url", null, "test blogId", _constructorParameters[3]};
                yield return new object[]
                    {"test title", "test content", null, "test userId", "test blogId", _constructorParameters[2]};
                yield return new object[]
                    {"test title", null, "test url", "test userId", "test blogId", _constructorParameters[1]};
                yield return new object[]
                    {null, "test content", "test url", "test userId", "test blogId", _constructorParameters[0]};
                yield return new object[]
                    {"test title", "test content", "test url", "test userId", "", _constructorParameters[4]};
                yield return new object[]
                    {"test title", "test content", "test url", "", "test blogId", _constructorParameters[3]};
                yield return new object[]
                    {"test title", "test content", "", "test userId", "test blogId", _constructorParameters[2]};
                yield return new object[] {"test title", "", "test url", "test userId", "test blogId", _constructorParameters[1]};
                yield return new object[]
                    {"", "test content", "test url", "test userId", "test blogId", _constructorParameters[0]};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}