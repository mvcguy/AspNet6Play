using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;

namespace PlayWebApp.Services.Logistics.LocationMgt.Repository
{
    public class LocationRepository : NavigationRepository<Address>
    {
        public LocationRepository(ApplicationDbContext dbConext) : base(dbConext)
        {
            
        }

        public override IQueryable<Address> GetTenantBasedQuery(string tenantId, bool includeSubItems = true)
        {
            return dbContext.Set<Address>().Where(x => x.TenantId == tenantId);
        }
    }
}