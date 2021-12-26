using IdentityServer4.Test;

namespace PlayConnectServer.CustomUserStore
{
    public static class AppIdentityExtentions
    {
        /// <summary>
        /// Adds test users.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="users">The users.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddAppUsers(this IIdentityServerBuilder builder)
        {
            builder.Services.AddScoped<AppUserStore>();
            builder.AddProfileService<AppUserProfileService>();
            builder.AddResourceOwnerValidator<AppUserPasswordValidator>();

            return builder;
        }
    }

}