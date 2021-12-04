using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.ViewModels;
#nullable disable

namespace PlayWebApp.Areas.Logistics.Pages.Booking
{
    public class ManageBookingModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public ManageBookingModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public BookingUpdateVm BookingVm { get; set; }

        public async Task OnGet()
        {
            var top1 = await dbContext.Bookings.OrderBy(x => x.BookingNumber).FirstOrDefaultAsync();
            if (top1 != null)
            {
                BookingVm = new BookingUpdateVm { BookingNumber = top1.BookingNumber, Description = top1.Description };
            }
        }
    }
}