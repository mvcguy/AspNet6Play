using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PlayWebApp.Services.Database.Model;

namespace PlayWebApp.Services.Database;

public class UserManagerExt : UserManager<IdentityUser>
{
    private readonly ApplicationDbContext dbContext;

    public UserManagerExt(IUserStore<IdentityUser> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<IdentityUser> passwordHasher,
    IEnumerable<IUserValidator<IdentityUser>> userValidators,
    IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<UserManager<IdentityUser>> logger, ApplicationDbContext dbContext) :
    base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        this.dbContext = dbContext;
    }

    public async Task<IdentityResult> CreateAsync(IdentityUser user, IdentityUserExt userExt, string password)
    {
        var baseResult = await base.CreateAsync(user, password);

        if (baseResult.Succeeded)
        {            
            dbContext.Add(userExt);
            dbContext.SaveChanges();
        }

        return baseResult;
    }
}

