using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Logistics.CustomerManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Logistics.BookingMgt;
using System.Linq;
using PlayWebApp.Services.Logistics.BookingMgt.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
using PlayWebApp.Services.ModelExtentions;


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
            BookingVm = new BookingUpdateVm
            {
                Lines = new List<BookingItemUpdateVm>(),
                LinesMetaData = new CollectionMetaData()
            };
            var top1 = await bookingService.GetFirst();
            if (top1 != null)
            {
                BookingVm = new BookingUpdateVm
                {
                    RefNbr = top1.RefNbr,
                    Description = top1.Description,
                    CustomerRefNbr = top1.CustomerRefNbr,
                };
            }

            if (!string.IsNullOrWhiteSpace(BookingVm.RefNbr))
            {
                var details = await bookingService.GetBookingLines(BookingVm.RefNbr, 1);
                if (details != null && details.Items != null && details.Items.Any())
                {
                    BookingVm.Lines = details.Items.Select(x => x.ToVm()).ToList();
                    BookingVm.LinesMetaData = details.MetaData;
                }
            }

            Customers = new List<SelectListItem>();
            Customers.Add(new SelectListItem { Text = "Select customer", Value = "", Selected = top1 == null });
            Customers.AddRange((await customerService.GetAll(page: 1))
                .Items.Select(x => new SelectListItem
                {
                    Value = x.RefNbr,
                    Text = $"{x.RefNbr} - {x.Name}",
                    Selected = top1?.CustomerRefNbr == x.RefNbr
                }));

        }
    }
}