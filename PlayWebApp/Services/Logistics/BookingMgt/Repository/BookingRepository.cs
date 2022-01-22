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

        public override IQueryable<Booking> GetQuery()
        {
            return dbContext.Set<Booking>().Where(x => x.TenantId == context.TenantId);
        }


    }

}