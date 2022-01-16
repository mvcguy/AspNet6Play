using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;

namespace PlayWebApp.Services.Logistics.CustomerManagement
{
    public class CustomerRepository : NavigationRepository<Customer>
    {
        public CustomerRepository(ApplicationDbContext dbContext, IPlayAppContext context) : base(dbContext, context)
        {
        }

        public override IQueryable<Customer> GetQueryByParentId(string parentId)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<Customer> GetTenantBasedQuery(bool includeSubItems = false)
        {
            if(includeSubItems)
            {
                return dbContext.Customers.Include(x => x.Addresses).Where(x => x.TenantId == context.TenantId);
            }

            return dbContext.Customers.Where(x => x.TenantId == context.TenantId);
        }
    }
}