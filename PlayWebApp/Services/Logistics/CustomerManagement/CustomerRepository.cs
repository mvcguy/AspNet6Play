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

        public override IQueryable<Customer> GetQuery()
        {
            var query = base.GetQuery();

            return query.Include(x => x.Addresses).Include(x => x.DefaultAddress);

        }
    }
}