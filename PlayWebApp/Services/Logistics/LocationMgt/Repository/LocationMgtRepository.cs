using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;

namespace PlayWebApp.Services.Logistics.LocationMgt.Repository
{
    public class CustomerLocationRepository : NavigationRepository<CustomerAddress>
    {
        public CustomerLocationRepository(ApplicationDbContext dbConext, IPlayAppContext context) : base(dbConext, context)
        {
            
        }

        public override IQueryable<CustomerAddress> GetTenantBasedQuery(bool includeSubItems = true)
        {
            return dbContext.CustomerAddresses.Where(x => x.TenantId == context.TenantId);
        }
    }

    public class SupplierLocationRepository : NavigationRepository<SupplierAddress>
    {
        public SupplierLocationRepository(ApplicationDbContext dbContext, IPlayAppContext context) : base(dbContext, context)
        {
        }

        public override IQueryable<SupplierAddress> GetTenantBasedQuery(bool includeSubItems = true)
        {
            return dbContext.SupplierAddresses.Where(x => x.TenantId == context.TenantId);
        }
    }
}