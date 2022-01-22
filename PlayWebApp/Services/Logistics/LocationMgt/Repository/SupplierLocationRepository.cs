using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;

namespace PlayWebApp.Services.Logistics.LocationMgt.Repository
{

    public class SupplierLocationRepository : NavigationRepository<SupplierAddress>
    {
        public SupplierLocationRepository(ApplicationDbContext dbContext, IPlayAppContext context) : base(dbContext, context)
        {
        }
        public override IQueryable<SupplierAddress> GetQuery()
        {
            return dbContext.SupplierAddresses.Where(x => x.TenantId == context.TenantId);
        }
    }
}