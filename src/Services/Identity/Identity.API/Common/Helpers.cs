using Identity.Data.Entities;

namespace Identity.API.Common
{
    public static class Helpers
    {
        public static string GetFullName(this ApplicationUser user)
        {
            return $"{user.FirstName} {user.LastName}";
        }
    }
}