using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PlayWebApp.Services.Database.Model;
using System.Linq;
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


    public virtual async Task<IdentityResult> UpdateExtendedUser(IdentityUserExt userExt)
    {
        try
        {
            var record = await GetUserExtAsync(userExt.UserId);

            record.FirstName = userExt.FirstName;
            record.LastName = userExt.LastName;
            record.DefaultAddressId = userExt.DefaultAddressId;
            record.ModifiedOn = DateTime.Now;


            dbContext.Set<IdentityUserExt>().Update(record);
            await dbContext.SaveChangesAsync();
            return IdentityResult.Success;
        }
        catch (Exception e)
        {
            return IdentityResult.Failed(new IdentityError() { Description = e.Message });
        }
    }

    public virtual async Task<IEnumerable<Address>> GetUserAddressesAsync(string userId)
    {
        return await dbContext.Set<Address>().Where(x => x.UserId == userId).ToListAsync();
    }

    public virtual async Task<Address> GetUserDefaultAddress(string userId, string addressId)
    {
        return await dbContext.Set<Address>().FirstOrDefaultAsync(x => x.UserId == userId && x.Id == addressId);
    }

    public virtual async Task<IdentityUserExt> GetUserExtAsync(string userId)
    {
        return await dbContext.Set<IdentityUserExt>().FirstOrDefaultAsync(x => x.UserId == userId);

    }

    public virtual async Task<Address> GetUserAddress(string userId, string addressCode)
    {
        return await dbContext.Addresses.FirstOrDefaultAsync(x => x.Code == addressCode && x.UserId == userId);
    }


}

