using Posting.API.Asp;
using Posting.Core.Interfaces.Asp;

namespace Posting.UnitTests.TestData
{
    public class TestAdminInfo : IUserInfo
    {
        public string Id => TestConstants.AdminId;
        public string FirstName => "Admin";
        public string LastName => "Test";
        public string Email => "jhonny.test@gmail.com";
        public string Name => $"{FirstName} {LastName}";
        public string Role => Roles.Administrator;

        public string[] OwnedBlogs
        {
            get { return new[] {TestConstants.BlogId}; }
        }
    }
}
