#nullable disable

using PlayWebApp;
using PlayWebApp.Services.Logistics.ViewModels;

namespace PlayWebApp.Services.Logistics.BookingMgt.ViewModels
{
    public class BookingItemDto : BaseDto
    {
        public virtual string BookingRefNbr { get; set; }

        public virtual string StockItemRefNbr { get; set; }

        public virtual string Description { get; set; }

        public virtual decimal Quantity { get; set; }

        public virtual decimal UnitCost { get; set; }

        public virtual decimal ExtCost { get; set; }

        public virtual decimal Discount { get; set; }

    }


}