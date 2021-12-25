using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.CustomerManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Logistics.ViewModels;
using System.Linq;
#nullable disable

namespace PlayWebApp.Areas.Logistics.Pages.Booking
{
    public class ManageBookingModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;
        private readonly CustomerService customerService;

        public ManageBookingModel(ApplicationDbContext dbContext, CustomerService customerService)
        {
            this.dbContext = dbContext;
            this.customerService = customerService;
        }

        public BookingUpdateVm BookingVm { get; set; }

        public List<SelectListItem> Customers { get; set; }

        public async Task OnGet()
        {
            var top1 = await dbContext.Bookings.OrderBy(x => x.RefNbr).FirstOrDefaultAsync();
            if (top1 != null)
            {
                BookingVm = new BookingUpdateVm { RefNbr = top1.RefNbr, Description = top1.Description };
            }

            Customers = (await customerService.GetAll(page: 1)).Select(x => new SelectListItem
            {
                Value = x.RefNbr,
                Text = $"{x.RefNbr} - {x.Name}"
            }).ToList();
        }
    }
}