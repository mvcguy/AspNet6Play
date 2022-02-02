using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;

#nullable disable

namespace PlayWebApp.Services.Logistics.InventoryMgt.Repository
{
    public class InventoryRepository : NavigationRepository<StockItem>
    {
        public InventoryRepository(ApplicationDbContext dbContext, IPlayAppContext context) : base(dbContext, context) { }

        public override IQueryable<StockItem> GetQuery()
        {
            return dbContext.Set<StockItem>().Where(x => x.TenantId == context.TenantId);
        }
        public async Task<PagedResult<StockItemPrice>> GetItemPricesPaginated(string refNbr, int pageLength, int page)
        {
            var query = dbContext.StockItemPrices.Where(x => x.RefNbr == refNbr && x.TenantId == context.TenantId);
            var count = await query.CountAsync();
            if (count == 0) return null;
            GetPagingInfo(page, pageLength, out var take, out var skip);
            var items = await query.Skip(skip).Take(take).ToListAsync();

            return new PagedResult<StockItemPrice>
            {
                PageIndex = page,
                PageSize = pageLength,
                Records = items,
                TotalRecords = count,
            };

        }

        public async Task<IEnumerable<StockItemPrice>> GetItemPrices(string refNbr)
        {
            var query = dbContext.StockItemPrices.Where(x => x.RefNbr == refNbr && x.TenantId == context.TenantId);
            return await query.ToListAsync();

        }
    }

}