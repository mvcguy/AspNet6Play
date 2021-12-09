using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PlayWebApp.Services.Database;
#nullable disable

namespace PlayWebApp.Services.Identity
{
    public class AdditionalUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<IdentityUser>
    {
        private readonly UserManagerExt userManagerExt;

        public AdditionalUserClaimsPrincipalFactory(
        UserManagerExt userManagerExt,
        ApplicationDbContext dbContext,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManagerExt, optionsAccessor)
        {
            this.userManagerExt = userManagerExt;
        }

        public async override Task<ClaimsPrincipal> CreateAsync(IdentityUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;
            var tentantId = (await userManagerExt.GetUserExtAsync(user.Id)).TenantId;

            var claims = new List<Claim>();
            // if (user.IsAdmin)
            // {
            //     claims.Add(new Claim(JwtClaimTypes.Role, "admin"));
            // }
            // else
            // {
            //     claims.Add(new Claim(JwtClaimTypes.Role, "user"));
            // }

            if (!string.IsNullOrWhiteSpace(tentantId))
            {
                claims.Add(new Claim(CustomClaimTypes.TenantId, tentantId.ToString()));
                identity.AddClaims(claims);
            }
            
            return principal;
        }
    }
}