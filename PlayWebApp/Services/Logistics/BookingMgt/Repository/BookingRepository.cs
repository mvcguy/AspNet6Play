using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
#nullable disable

namespace PlayWebApp.Services.Logistics.BookingMgt.Repository
{
    public class BookingRepository : NavigationRepository<Booking>
    {
        public BookingRepository(ApplicationDbContext dbContext, IPlayAppContext context) : base(dbContext, context) { }

        public override IQueryable<Booking> GetQueryByParentId(string parentId)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<Booking> GetTenantBasedQuery(bool includeSubItems = true)
        {
            var query = dbContext.Set<Booking>().Where(x => x.TenantId == context.TenantId);
            if (includeSubItems)
            {
                return query.Include(x => x.BookingItems);
            }
            return query;
        }


    }

}