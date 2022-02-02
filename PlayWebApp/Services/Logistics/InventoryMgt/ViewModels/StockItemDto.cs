#nullable disable
using PlayWebApp;
using PlayWebApp.Services.Logistics.ViewModels;

namespace PlayWebApp.Services.Logistics.InventoryMgt.ViewModels
{
    public class StockItemDto : BaseDto
    {
        public string ItemDescription { get; set; }

        public IList<StockItemPriceDto> Prices { get; set; }
    }

}