using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlayWebApp.Services.Logistics.CustomerManagement;
using PlayWebApp.Services.Logistics.BookingMgt;
using PlayWebApp.Services.Logistics.BookingMgt.ViewModels;
using PlayWebApp.Services.ModelExtentions;
using PlayWebApp.Services.Logistics.ViewModels;

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

        public async Task OnGet(string refNbr = null)
        {
            BookingVm = new BookingUpdateVm
            {
                Lines = new List<BookingItemUpdateVm>(),
                LinesMetaData = new CollectionMetaData()
            };
            if (!string.IsNullOrWhiteSpace(refNbr))
            {
                var rec = await bookingService.GetById(new BookingRequestDto { RefNbr = refNbr });
                if (rec != null)
                    BookingVm = rec.ToVm();
            }
            else
            {
                var top1 = await bookingService.GetFirst();
                if (top1 != null)
                {
                    BookingVm = top1.ToVm();
                }
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
            Customers.Add(new SelectListItem { Text = "Select customer", Value = "", 
                    Selected = BookingVm.CustomerRefNbr == null });
            Customers.AddRange((await customerService.GetAll(page: 1))
                .Items.Select(x => new SelectListItem
                {
                    Value = x.RefNbr,
                    Text = $"{x.RefNbr} - {x.Name}",
                    Selected = BookingVm.CustomerRefNbr == x.RefNbr
                }));

        }
    }
}