using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;

namespace PlayWebApp.Services.Logistics.CustomerManagement
{
    public class CustomerLocationRepository : NavigationRepository<CustomerAddress>
    {
        public CustomerLocationRepository(ApplicationDbContext dbConext, IPlayAppContext context)
         : base(dbConext, context)
        {

        }
        public override IQueryable<CustomerAddress> GetQuery()
        {
            return dbContext.CustomerAddresses.Where(x => x.TenantId == context.TenantId);
        }

        

    }
}