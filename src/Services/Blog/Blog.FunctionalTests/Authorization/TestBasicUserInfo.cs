﻿using Blog.API.Asp;
using Blog.API.Common;

namespace Blog.FunctionalTests.Authorization
{
    public class TestBasicUserInfo : IUserInfo
    {
        public string Id => TestConstants.UserId;
        public string FirstName => "Jhonny";
        public string LastName => "Test";
        public string Email => "jhonny.test@gmail.com";
        public string Name => $"{(FirstName ?? "?")[0]}.{LastName}";
        public string Role => Roles.BasicUser;
    }
}