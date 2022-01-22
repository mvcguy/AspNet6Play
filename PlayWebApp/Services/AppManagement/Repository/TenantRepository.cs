using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
#nullable disable

namespace PlayWebApp.Services.AppManagement.Repository
{
    public class TenantRepository : NavigationRepository<Tenant>
    {
        public TenantRepository(ApplicationDbContext dbContext, IPlayAppContext context) : base(dbContext, context)
        {
        }

        public override IQueryable<Tenant> GetQuery()
        {
            return dbContext.Tenants;
        }

        public override void ThrowOnEmptyContext()
        {
            /* ignore for this repository */
        }
    }
}