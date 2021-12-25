#nullable disable
using PlayWebApp;

namespace PlayWebApp.Services.Logistics.ViewModels.Dtos
{
    public class BookingDto : BaseDto
    {
        public string Description { get; set; }

        public IList<BookingItemDto> Lines { get; set; }
    }


}