#nullable disable
using PlayWebApp;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;

namespace PlayWebApp.Services.Logistics.BookingMgt.ViewModels
{
    public class BookingDto : BaseDto
    {

        public string CustomerRefNbr { get; set; }

        public string Description { get; set; }

        public IList<BookingItemDto> Lines { get; set; }
    }


}