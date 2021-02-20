using Blogging.API.Asp;

namespace Blogging.IntegrationTests.Authorization
{
    public class TestAdminInfo : IUserInfo
    {
        public string Id => TestConstants.AdminId;
        public string FirstName => "Admin";
        public string LastName => "Test";
        public string Email => "jhonny.test@gmail.com";
        public string Name => $"{FirstName} {LastName}";
        public string Role => Roles.Administrator;
        public string[] OwnedBlogs => new[] {TestConstants.BlogId};
    }
}