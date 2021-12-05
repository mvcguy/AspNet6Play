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
            dbContext.Set<IdentityUserExt>().Update(userExt);
            await dbContext.SaveChangesAsync();
            return IdentityResult.Success;
        }
        catch (Exception e)
        {
            return IdentityResult.Failed(new IdentityError() { Description = e.Message });
        }
    }

    public virtual async Task<IdentityUserExt> GetUserExtWithAddressesAsync(IdentityUser user)
    {
        return await dbContext.Set<IdentityUserExt>().Include(x => x.Addresses).FirstOrDefaultAsync(x => x.UserId == user.Id);
    }

    public virtual async Task<IdentityUserExt> GetUserExtAsync(IdentityUser user, bool includeDefaultAddress = false)
    {

        IQueryable<IdentityUserExt> query;
        if (includeDefaultAddress)
        {
            query = from u in dbContext.Set<IdentityUserExt>()
                    join a in dbContext.Addresses on new { k1 = u.UserId, k2 = u.DefaultAddressId } equals new { k1 = a.UserId, k2 = a.Id }
                    select new IdentityUserExt
                    {
                        UserId = u.UserId,
                        DefaultAddressId = u.DefaultAddressId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Addresses = new List<Address> { a }
                    } into UserAndAddres
                    select UserAndAddres;
        }
        else
        {
            query = dbContext.Set<IdentityUserExt>();
        }


        var userExt = await query.FirstOrDefaultAsync(x => x.UserId == user.Id);
        return userExt;
    }

    public virtual async Task<Address> GetUserAddress(IdentityUserExt userExt, string addressCode)
    {
        return await dbContext.Addresses.FirstOrDefaultAsync(x => x.AddressCode == addressCode);
    }


}

