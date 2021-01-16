using Microsoft.AspNetCore.Http;

namespace Blogging.API.Asp
{
  public class AspUserInfo : UserInfo
  {
    public AspUserInfo(IHttpContextAccessor httpContextAccessor)
      : base(httpContextAccessor?.HttpContext?.User)
    {
    }
  }
}