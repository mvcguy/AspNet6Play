#nullable disable
using PlayWebApp;

namespace PlayWebApp.Services.Logistics.ViewModels.Dtos
{
    public class StockItemDto : BaseDto
    {
        public string ItemDescription { get; set; }

        public IList<StockItemPriceDto> Prices { get; set; }
    }

}