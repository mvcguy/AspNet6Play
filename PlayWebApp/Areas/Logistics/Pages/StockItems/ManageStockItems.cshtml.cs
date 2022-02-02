using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Logistics.InventoryMgt;
using PlayWebApp.Services.Logistics.InventoryMgt.ViewModels;
using PlayWebApp.Services.ModelExtentions;
#nullable disable
namespace PlayWebApp.Areas.Logistics.Pages.StockItems
{
    public class ManageStockItemsModel : PageModel
    {
        private readonly InventoryService service;

        public ManageStockItemsModel(InventoryService service)
        {
            this.service = service;
        }

        [BindProperty]
        public StockItemUpdateVm StockItemVm { get; set; }

        public async Task OnGetAsync(string refNbr = null)
        {
            StockItemVm = new StockItemUpdateVm { ItemPrices = new List<StockItemPriceUpdateVm>() };
            if (!string.IsNullOrWhiteSpace(refNbr))
            {
                var rec = await service.GetById(new StockItemRequestDto { RefNbr = refNbr });
                if (rec != null)
                    StockItemVm = rec.ToVm();
            }
            else
            {
                var top1 = await service.GetFirst();
                if (top1 != null)
                {
                    StockItemVm = top1.ToVm();
                }
            }

            if (!string.IsNullOrWhiteSpace(StockItemVm.RefNbr))
            {
                var prices = await service.GetItemPrices(StockItemVm.RefNbr, 1);
                if (prices != null && prices.Items != null && prices.Items.Any())
                {
                    StockItemVm.ItemPrices = prices.Items.Select(x => x.ToVm()).ToList();
                    StockItemVm.ItemPricesMetaData = prices.MetaData;
                }
            }
        }

    }
}