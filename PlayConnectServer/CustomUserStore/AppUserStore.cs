using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Test;

namespace PlayConnectServer.CustomUserStore
{

    public class AppUserStore
    {
        private readonly AppDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppUserStore"/> class.
        /// </summary>
        /// <param name="users">The users.</param>
        public AppUserStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Validates the credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public bool ValidateCredentials(string username, string password)
        {
            var user = FindByUsername(username);

            if (user != null)
            {
                // TODO: introduce real validation using sec-hash, sec-tstamp & sec-salt
                return true;
            }

            return false;
        }

        /// <summary>
        /// Finds the user by subject identifier.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <returns></returns>
        public TestUser FindBySubjectId(string subjectId)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Id == subjectId);
            return ToTestUser(user);
        }

        /// <summary>
        /// Finds the user by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public TestUser FindByUsername(string username)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.UserName == username);
            return ToTestUser(user);
        }

        
        private TestUser ToTestUser(ApplicationUser appUser)
        {
            if (appUser == null) return null;
            var address = new
            {
                street_address = "One Hacker Way",
                locality = "Islamabad",
                postal_code = 69118,
                country = "Pakistan"
            };
            return new TestUser
            {
                SubjectId = appUser.Id,
                IsActive = true,
                Username = appUser.UserName,
                Claims =
                        {
                            new Claim("PlayWebApp.TenantId", appUser.TenantId),
                            new Claim(JwtClaimTypes.Name, $"{appUser.FirstName} {appUser.LastName}"),
                            new Claim(JwtClaimTypes.GivenName, appUser.FirstName),
                            new Claim(JwtClaimTypes.FamilyName, appUser.LastName),
                            new Claim(JwtClaimTypes.Email, appUser.Email),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "https://www.shahid.ali.star"),
                            new Claim("Children", "pluto, jupitor"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json)
                        }
            };
        }

        internal TestUser FindByExternalProvider(string provider, string providerUserId)
        {
            throw new NotImplementedException();
        }

        internal TestUser AutoProvisionUser(string provider, string providerUserId, List<Claim> claims)
        {
            throw new NotImplementedException();
        }
    }

}