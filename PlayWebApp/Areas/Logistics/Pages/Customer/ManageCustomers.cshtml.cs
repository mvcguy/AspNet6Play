using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayWebApp.Services.CustomerManagement;
using PlayWebApp.Services.CustomerManagement.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.ModelExtentions;
#nullable disable

namespace PlayWebApp.Areas.Logistics.Pages.Booking
{
    public class ManageCustomersModel : PageModel
    {
        private readonly CustomerService customerService;

        public ManageCustomersModel(CustomerService customerService)
        {
            this.customerService = customerService;
        }

        public CustomerUpdateVm CustomerVm { get; set; }

        public async Task OnGet(string refNbr = null)
        {
            CustomerVm = new CustomerUpdateVm { Addresses = new List<AddressUpdateVm>() };
            if (!string.IsNullOrWhiteSpace(refNbr))
            {
                var rec = await customerService.GetById(new CustomerRequestDto { RefNbr = refNbr });
                if (rec != null)
                    CustomerVm = rec.ToVm();
            }
            else
            {
                var top1 = await customerService.GetFirst();
                if (top1 != null)
                {
                    CustomerVm = top1.ToVm();
                }
            }
        }
    }
}