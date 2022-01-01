using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.CustomerManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Logistics.BookingMgt;
using PlayWebApp.Services.Logistics.ViewModels;
using System.Linq;
#nullable disable

namespace PlayWebApp.Areas.Logistics.Pages.Booking
{
    public class ManageBookingModel : PageModel
    {
        private readonly BookingService bookingService;
        private readonly CustomerService customerService;

        public ManageBookingModel(BookingService bookingService, CustomerService customerService)
        {
            this.bookingService = bookingService;
            this.customerService = customerService;
        }

        public BookingUpdateVm BookingVm { get; set; }

        public List<SelectListItem> Customers { get; set; }

        public async Task OnGet()
        {
            var top1 = await bookingService.GetFirst();
            if (top1 != null)
            {
                BookingVm = new BookingUpdateVm { RefNbr = top1.RefNbr, Description = top1.Description };
            }
            Customers = new List<SelectListItem>();
            Customers.Add(new SelectListItem { Text = "Select customer", Value = "", Selected = true });
            Customers.AddRange((await customerService.GetAll(page: 1)).Select(x => new SelectListItem
            {
                Value = x.RefNbr,
                Text = $"{x.RefNbr} - {x.Name}",
            }));

        }
    }
}