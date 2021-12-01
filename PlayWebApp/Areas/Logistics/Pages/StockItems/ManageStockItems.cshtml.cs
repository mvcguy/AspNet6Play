using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Logistics.Model;
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
            await Task.FromResult<int>(0);
            StockItemsList = dbContext.StockItems.ToList();
        }

    }
}