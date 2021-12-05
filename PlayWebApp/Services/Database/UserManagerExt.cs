using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PlayWebApp.Services.Database.Model;
#nullable disable
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

    public virtual async Task<IdentityResult> UpdateExtendedUser(IdentityUserExt userExt)
    {        
        try
        {
            dbContext.Set<IdentityUserExt>().Update(userExt);
            await dbContext.SaveChangesAsync();
            return IdentityResult.Success;
        }
        catch (Exception e)
        {
            return IdentityResult.Failed(new IdentityError() { Description = e.Message });
        }
    }

    public virtual async Task<IdentityUserExt> GetUserExtAsync(IdentityUser user)
    {
        var userExt = await dbContext.Set<IdentityUserExt>().FirstOrDefaultAsync(x => x.UserId == user.Id);
        return userExt;
    }
}

