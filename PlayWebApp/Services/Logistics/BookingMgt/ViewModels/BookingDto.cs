#nullable disable
using PlayWebApp;
using PlayWebApp.Services.Logistics.ViewModels;

namespace PlayWebApp.Services.Logistics.BookingMgt.ViewModels
{
    public class BookingDto : BaseDto
    {

        public string CustomerRefNbr { get; set; }

        public string Description { get; set; }

        public virtual decimal LinesTotal { get; set; }

        public virtual decimal TotalAmount { get; set; }

        public virtual decimal Balance { get; set; }

        public virtual decimal Discount { get; set; }

        public virtual decimal TaxableAmount { get; set; }

        public virtual decimal TaxAmount { get; set; }

        public IList<BookingItemDto> Lines { get; set; }
    }


}