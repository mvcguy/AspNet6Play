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
            if (result > 0) return OperationResult.Success();
            return OperationResult.Failure(new Exception("The update did not affect any rows"));
        }

        public OperationResult CreateUser(ApplicationUser appUser)
        {
            try
            {
                AppUsers.Add(appUser);
                return OperationResult.Success(appUser.Id);
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
                return OperationResult.Success(appUser.Id);
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


    }


}