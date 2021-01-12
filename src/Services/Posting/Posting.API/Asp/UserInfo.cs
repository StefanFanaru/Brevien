using System.Security.Claims;
using Posting.Core.Interfaces.Asp;

namespace Posting.API.Asp
{
    public class UserInfo : IUserInfo
    {
        public UserInfo(ClaimsPrincipal user)
        {
            if (user != null)
            {
                Id = user.FindFirstValue(Claims.UserId);
                FirstName = user.FindFirstValue(Claims.FirstName);
                LastName = user.FindFirstValue(Claims.LastName);
                Email = user.FindFirstValue(Claims.Email);
                Role = user.FindFirstValue(Claims.Role);
            }
        }

        public string Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Role { get; }
        public string Name => $"{(FirstName ?? "?")[0]}.{LastName}";
    }
}