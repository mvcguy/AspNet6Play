using System.Security.Claims;
using PlayWebApp.Services.Identity;

#nullable disable
namespace PlayWebApp.Services.AppManagement
{
    public class PlayAppContext : IPlayAppContext
    {
        
        public string UserId { get; private set; }
        public string TenantId { get; private set; }

        public PlayAppContext(IHttpContextAccessor contextAccessor)
        {
            try
            {
                this.UserId = contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                this.TenantId = contextAccessor.HttpContext.User.FindFirstValue(CustomClaimTypes.TenantId);
            }
            catch { /*ignore*/}
        }
    }

}