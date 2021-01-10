using Blog.API.Asp;

namespace Blog.IntegrationTests.Authorization
{
    public class TestAdminInfo : IUserInfo
    {
        public string Id => TestConstants.AdminId;
        public string FirstName => "Admin";
        public string LastName => "Test";
        public string Email => "jhonny.test@gmail.com";
        public string Name => $"{(FirstName ?? "?")[0]}.{LastName}";
        public string Role => Roles.Administrator;
    }
}