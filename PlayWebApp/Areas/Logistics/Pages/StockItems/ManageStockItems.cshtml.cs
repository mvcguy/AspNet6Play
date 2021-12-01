// using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Logistics.Model;
#nullable disable
namespace PlayWebApp.Areas.Logistics.Pages.StockItems
{

    public class StockItemVm
    {
        [Required]
        [Display(Name = "Item ID:")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string ItemDisplayId { get; set; }

        [Required]
        [Display(Name = "Description")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string ItemDescription { get; set; }


    }

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
        public StockItemVm StockItemVm { get; set; }

        public IEnumerable<StockItem> StockItemsList { get; private set; }

        public async Task OnGetAsync()
        {
            var task = Task.Run(() =>
            {
                StockItemsList = dbContext.StockItems.Take(10).ToList();
            });

            await task;

        }

        public async Task<IActionResult> OnPostStockItemAsync()
        {
            // save changes in the database

            // return the created item

            await Task.FromResult<int>(0);

            return new JsonResult(StockItemVm);
        }
    }
}