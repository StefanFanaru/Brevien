using System;

namespace Blog.FunctionalTests
{
    public class TestConstants
    {
        public const string UserId = "c0c0dd27-2292-4a4b-a06b-52af7cc3a8b8";
        public const string BlogId = "5fe7885a6d02ecc81c87ade0";
        public const string AnotherUsersBlogId = "ef1891affb24a66d618951c4";
        public const string DisabledBlogId = "96c568a7421d5ad33962098b";
        public const string SoftDeletedBlogId = "6439e7236865cd003b09d588";
        public static readonly DateTime BlogCreatedAt = new(2020, 12, 29, 12, 1, 1, 123);
    }
}