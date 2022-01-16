using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;

namespace PlayWebApp.Services.Logistics.CustomerManagement
{
    public class CustomerLocationRepository : NavigationRepository<CustomerAddress>
    {
        public CustomerLocationRepository(ApplicationDbContext dbConext, IPlayAppContext context) : base(dbConext, context)
        {
            
        }

        public override IQueryable<CustomerAddress> GetQueryByParentId(string parentId)
        {
            return GetTenantBasedQuery().Where(x => x.Customer.RefNbr == parentId);

        }

        public override IQueryable<CustomerAddress> GetTenantBasedQuery(bool includeSubItems = true)
        {
            return dbContext.CustomerAddresses.Where(x => x.TenantId == context.TenantId);
        }
    }
}