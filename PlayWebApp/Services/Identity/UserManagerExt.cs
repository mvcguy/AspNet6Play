using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
#nullable disable
namespace PlayWebApp.Services.Identity;

public class UserManagerExtObsolete : UserManager<IdentityUser>
{
    private readonly ApplicationDbContext dbContext;

    public UserManagerExtObsolete(IUserStore<IdentityUser> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<IdentityUser> passwordHasher,
    IEnumerable<IUserValidator<IdentityUser>> userValidators,
    IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    UserManagementService userManagementService,
    ILogger<UserManager<IdentityUser>> logger, ApplicationDbContext dbContext) :
    base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        this.dbContext = dbContext;
    }

    public virtual async Task<IdentityResult> CreateAsync(IdentityUser user, IdentityUserExt userExt, string password)
    {
        var baseResult = await base.CreateAsync(user, password);

        if (baseResult.Succeeded)
        {
            dbContext.Add(userExt);
            dbContext.SaveChanges();
        }

        return baseResult;
    }

    public virtual async Task<IdentityResult> CreateAsync(IdentityUser user, IdentityUserExt userExt, Address address, string password)
    {
        var baseResult = await base.CreateAsync(user, password);

        if (baseResult.Succeeded)
        {
            dbContext.Addresses.Add(address);
            dbContext.Add(userExt);
            dbContext.SaveChanges();
        }

        return baseResult;
    }

}

