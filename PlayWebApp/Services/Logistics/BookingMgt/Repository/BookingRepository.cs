using Microsoft.EntityFrameworkCore;
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
            return dbContext.Set<Booking>()
                .Include(x => x.Customer)
                .Include(x => x.ShippingAddress).Where(x => x.TenantId == context.TenantId);
        }

        public async Task<PagedResult<BookingItem>> GetBookingLinesPaginated(string bookingRef, int pageLength, int page)
        {
            var query = dbContext.Set<BookingItem>().Include(x=>x.StockItem).Where(x => x.Booking.RefNbr == bookingRef && x.TenantId == context.TenantId);
            var count = await query.CountAsync();
            if (count == 0) return null;
            GetPagingInfo(page, pageLength, out var take, out var skip);
            var items = await query.Skip(skip).Take(take).ToListAsync();

            return new PagedResult<BookingItem>
            {
                PageIndex = page,
                PageSize = pageLength,
                Records = items,
                TotalRecords = count,
            };

        }

        public async Task<IEnumerable<BookingItem>> GetBookingLines(string bookingRef)
        {
            var query = dbContext.Set<BookingItem>().Where(x => x.Booking.RefNbr == bookingRef && x.TenantId == context.TenantId);
            return await query.ToListAsync();

        }


    }

}