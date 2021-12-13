using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Identity;
#nullable disable

namespace PlayWebApp.Controllers
{
    [Authorize]
    [ApiController]
    public class BaseController : Controller
    {
        protected virtual string UserId
        {
            get
            {
                try
                {
                    return User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                catch
                {
                    return null;
                }

            }
        }

        protected virtual string TenantId
        {
            get
            {
                try
                {
                    return User.FindFirstValue(CustomClaimTypes.TenantId);
                }
                catch
                {
                    return null;
                }
            }
        }


        // protected virtual string DefaultAddressId
        // {
        //     get
        //     {
        //         try
        //         {
        //             var userExt = dbContext.Set<IdentityUserExt>().FirstOrDefault(x => x.UserId == UserId);
        //             return userExt?.DefaultAddressId;
        //         }
        //         catch
        //         {
        //             return null;
        //         }
        //     }
        // }

    }
}