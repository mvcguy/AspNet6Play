#nullable disable
using PlayWebApp;
using PlayWebApp.Services.Logistics.ViewModels;

namespace PlayWebApp.Services.Logistics.BookingMgt.ViewModels
{
    public class BookingItemUpdateVm : ViewModelBase
    {
        public new string RefNbr { get; set; }
        public string Description { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ExtCost { get; set; }
        public decimal? Quantity { get; set; }
        public string StockItemId { get; set; }
        public decimal? UnitCost { get; set; }
        public string UserId { get; set; }
    }
}