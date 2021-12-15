using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;

namespace PlayWebApp.Services.Logistics.LocationMgt.Repository
{
    public class LocationRepository : NavigationRepository<Address>
    {
        public LocationRepository(ApplicationDbContext dbConext, IPlayAppContext context) : base(dbConext, context)
        {
            
        }

        public override IQueryable<Address> GetTenantBasedQuery(bool includeSubItems = true)
        {
            return dbContext.Set<Address>().Where(x => x.TenantId == context.TenantId);
        }
    }
}