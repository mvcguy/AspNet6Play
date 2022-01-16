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
    {        public InventoryRepository(ApplicationDbContext dbContext, IPlayAppContext context): base(dbContext, context){}

        public override IQueryable<StockItem> GetQueryByParentId(string parentId)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<StockItem> GetTenantBasedQuery(bool includePrices = true)
        {
            var query = dbContext.Set<StockItem>().Where(x => x.TenantId == context.TenantId);
            if (includePrices)
            {
                return query.Include(x => x.StockItemPrices);
            }
            return query;
        }



    }

}