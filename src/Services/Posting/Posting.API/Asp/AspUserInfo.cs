using Microsoft.AspNetCore.Http;

namespace Posting.API.Asp
{
    public class AspUserInfo : UserInfo
    {
        public AspUserInfo(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor?.HttpContext?.User)
        {
        }
    }
}