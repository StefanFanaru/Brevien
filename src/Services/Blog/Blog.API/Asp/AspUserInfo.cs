using Microsoft.AspNetCore.Http;
using Serilog;

namespace Blog.API.Asp
{
    public class AspUserInfo : UserInfo
    {
        public AspUserInfo(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor?.HttpContext?.User)
        {
            Log.Debug(httpContextAccessor.HttpContext.User.ToString());
        }
    }
}
