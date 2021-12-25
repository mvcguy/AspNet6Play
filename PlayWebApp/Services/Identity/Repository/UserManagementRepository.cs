using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.GenericModels;
#nullable disable

namespace PlayWebApp.Services.Identity.Repository
{

    public class UserRepository : NavigationRepository<ApplicationUser>
    {
        public UserRepository(ApplicationDbContext dbContext, IPlayAppContext context) : base(dbContext, context)
        {
        }

        public override IQueryable<ApplicationUser> GetTenantBasedQuery(bool includeSubItems = true)
        {
            return dbContext.Users.Where(x => x.TenantId == context.TenantId);
        }

        public override void ThrowOnEmptyContext()
        {
            /*ignore*/
        }
    }
}