#nullable disable

using PlayWebApp;

namespace PlayWebApp.Services.Logistics.ViewModels.Dtos
{
    public class BookingItemDto : BaseDto
    {
        public virtual string BookingNbr { get; set; }

        public virtual string StockItemId { get; set; }

        public virtual string Description { get; set; }

        public virtual string BookingId { get; set; }

        public virtual decimal Quantity { get; set; }

        public virtual decimal UnitCost { get; set; }

        public virtual decimal ExtCost { get; set; }

        public virtual decimal Discount { get; set; }

    }


}