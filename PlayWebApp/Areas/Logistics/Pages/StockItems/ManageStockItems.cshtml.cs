using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.ViewModels;
#nullable disable
namespace PlayWebApp.Areas.Logistics.Pages.StockItems
{


    public class ManageStockItemsModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public ManageStockItemsModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            StockItemsList = new List<StockItem>();
        }
        public string Welcome { get; set; } = "Hello from stockitems";

        [BindProperty]
        public StockItemUpdateVm StockItemVm { get; set; }

        public IEnumerable<StockItem> StockItemsList { get; set; }

        public async Task OnGetAsync()
        {
            StockItemsList = await dbContext.StockItems.OrderBy(x => x.DisplayId).ToListAsync();
            var item = await dbContext.StockItems.OrderBy(x => x.DisplayId).Take(1).FirstOrDefaultAsync();
            if (item != null)
            {
                StockItemVm = new StockItemUpdateVm
                {
                    ItemDisplayId = item.DisplayId,
                    ItemDescription = item.Description
                };
            }
        }

    }
}