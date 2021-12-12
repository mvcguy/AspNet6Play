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
        private readonly UserManager<IdentityUser> userManager;
        private readonly UserManagementService userManagementService;
        private readonly ApplicationDbContext dbContext;
        private readonly IOptions<IdentityOptions> optionsAccessor;

        public AdditionalUserClaimsPrincipalFactory(
        UserManager<IdentityUser> userManager,
        UserManagementService userManagementService,
        ApplicationDbContext dbContext,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
        {
            this.userManager = userManager;
            this.userManagementService = userManagementService;
            this.dbContext = dbContext;
            this.optionsAccessor = optionsAccessor;
        }

        public async override Task<ClaimsPrincipal> CreateAsync(IdentityUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;
            var tentantId = (await userManagementService.GetIdentityUserExt(user.Id))?.TenantId;

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