#nullable disable
namespace PlayWebApp.Services.Logistics.ViewModels
{
    public class BookingItemUpdateVm : ViewModelBase
    {
        public string BookingRefNbr { get; set; }
        public string Description { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ExtCost { get; set; }
        public decimal? Quantity { get; set; }
        public string StockItemId { get; set; }
        public decimal? UnitCost { get; set; }
        public string UserId { get; set; }
    }
}