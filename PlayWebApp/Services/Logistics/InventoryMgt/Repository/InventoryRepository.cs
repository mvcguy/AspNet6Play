using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;

#nullable disable

namespace PlayWebApp.Services.Logistics.InventoryMgt.Repository
{
    public class InventoryRepository : NavigationRepository<StockItem>
    {        public InventoryRepository(ApplicationDbContext dbContext): base(dbContext){}

        public override IQueryable<StockItem> GetTenantBasedQuery(string tenantId, bool includePrices = true)
        {
            var query = dbContext.Set<StockItem>().Where(x => x.TenantId == tenantId);
            if (includePrices)
            {
                return query.Include(x => x.StockItemPrices);
            }
            return query;
        }



    }

}