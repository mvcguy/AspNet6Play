using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.GenericModels;
#nullable disable

namespace PlayWebApp.Services.Identity.Repository
{

    public class UserManagementRepository
    {
        private readonly ApplicationDbContext dbContext;

        private DbSet<ApplicationUser> AppUsers;
        private DbSet<Address> Addresses;

        public UserManagementRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            AppUsers = this.dbContext.Set<ApplicationUser>();
            Addresses = this.dbContext.Set<Address>();
        }

        public async Task<OperationResult> SaveChanges()
        {
            var result = await dbContext.SaveChangesAsync();
            if (result > 0) return OperationResult.Success;
            return OperationResult.Failure(new Exception("The update did not affect any rows"));
        }

        #region User operations

        public OperationResult CreateUser(ApplicationUser appUser)
        {
            try
            {
                AppUsers.Add(appUser);
                return OperationResult.Success;
            }
            catch (Exception e)
            {
                return OperationResult.Failure(e);
            }

        }

        public OperationResult UpdateUser(ApplicationUser appUser)
        {
            try
            {
                appUser.ModifiedOn = DateTime.UtcNow;
                AppUsers.Update(appUser);
                return OperationResult.Success;
            }
            catch (Exception e)
            {
                return OperationResult.Failure(e);
            }

        }

        public async Task<ApplicationUser> GetUser(string userId)
        {
            return await AppUsers.FirstOrDefaultAsync(x => x.Id == userId);
        }

        #endregion User Operations

        #region User Address operations

        public async Task<IEnumerable<Address>> GetAllUserAddresses(string userId)
        {
            var query = from u in AppUsers
                        join a in Addresses on new { k1 = u.TenantId, k2 = u.CreatedBy }
                        equals new { k1 = a.TenantId, k2 = a.CreatedBy}
                        where u.CreatedBy == userId
                        select a;
            return await query.ToListAsync();
        }

        public async Task<Address> GetUserDefaultAddress(string userId)
        {

            var query = from u in AppUsers
                        join a in Addresses on new { k1 = u.TenantId, k2 = u.CreatedBy, k3 = u.DefaultAddressId }
                        equals new { k1 = a.TenantId, k2 = a.CreatedBy, k3 = a.Id }
                        where u.CreatedBy == userId
                        select a;


            var address = await query.FirstOrDefaultAsync();
            return address;
        }

        public async Task<Address> GetUserAddressById(string userId, string addressId)
        {
            return await Addresses.FirstOrDefaultAsync(x => x.Id == addressId && x.CreatedBy == userId);
        }

        public OperationResult UpdateUserAddress(Address address)
        {
            try
            {
                address.ModifiedOn = DateTime.UtcNow;
                Addresses.Update(address);
                return OperationResult.Success;
            }
            catch (Exception e)
            {
                return OperationResult.Failure(e);
            }
        }

        public OperationResult CreateUserAddress(Address address)
        {
            try
            {
                Addresses.Add(address);
                return OperationResult.Success;
            }
            catch (Exception e)
            {
                return OperationResult.Failure(e);
            }
        }

        public async Task<Address> GetUserAddressByCode(string userId, string addressCode)
        {
            return await Addresses.FirstOrDefaultAsync(x => x.Code == addressCode && x.CreatedBy == userId);
        }
    }
    #endregion User Address Operations

}