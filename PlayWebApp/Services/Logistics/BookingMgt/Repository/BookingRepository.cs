using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
#nullable disable

namespace PlayWebApp.Services.Logistics.BookingMgt.Repository
{
    public class BookingRepository : NavigationRepository<Booking>
    {
        public BookingRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public override IQueryable<Booking> GetTenantBasedQuery(string tenantId, bool includeSubItems = true)
        {
            var query = dbContext.Set<Booking>().Where(x => x.TenantId == tenantId);
            if (includeSubItems)
            {
                return query.Include(x => x.BookingItems);
            }
            return query;
        }


    }

}