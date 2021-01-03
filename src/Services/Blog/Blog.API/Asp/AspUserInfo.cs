using Microsoft.AspNetCore.Http;

namespace Blog.API.Asp
{
    public class AspUserInfo : UserInfo
    {
        public AspUserInfo(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor?.HttpContext?.User)
        {
        }
    }
}