using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;

namespace PlayWebApp.Services.CustomerManagement
{
    public class CustomerRepository : NavigationRepository<Customer>
    {
        public CustomerRepository(ApplicationDbContext dbContext, IPlayAppContext context) : base(dbContext, context)
        {
        }

        public override IQueryable<Customer> GetTenantBasedQuery(bool includeSubItems = true)
        {
            return dbContext.Customers.Where(x => x.TenantId == context.TenantId);
        }
    }
}