using Blog.API.Asp;
using Blog.API.Common;

namespace Blog.FunctionalTests.Authorization
{
    public class TestUserInfo : IUserInfo
    {
        private string? _id;
        private string _role;

        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    return TestConstants.UserId;
                }

                return _id;
            }

            set => _id = value;
        }

        public string FirstName => "Jhonny";
        public string LastName => "Test";
        public string Email => "jhonny.test@gmail.com";
        public string Name => $"{(FirstName ?? "?")[0]}.{LastName}";

        public string Role
        {
            get
            {
                if (string.IsNullOrEmpty(_role))
                {
                    return Roles.Administrator;
                }

                return _role;
            }
            set => _role = value;
        }
    }
}