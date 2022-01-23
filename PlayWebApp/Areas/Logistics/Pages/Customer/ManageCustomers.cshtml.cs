using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayWebApp.Services.Logistics.CustomerManagement;
using PlayWebApp.Services.Logistics.CustomerManagement.ViewModels;
using PlayWebApp.Services.Logistics.LocationMgt.ViewModels;
using PlayWebApp.Services.ModelExtentions;
#nullable disable

namespace PlayWebApp.Areas.Logistics.Pages.Booking
{
    public class ManageCustomersModel : PageModel
    {
        private readonly CustomerService customerService;
        private readonly CustomerLocationService locService;

        public ManageCustomersModel(CustomerService customerService, CustomerLocationService locService)
        {
            this.customerService = customerService;
            this.locService = locService;
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

            if (!string.IsNullOrWhiteSpace(CustomerVm.RefNbr))
            {
                var addresses = await locService
                    .GetAllByCustomerId(CustomerVm.RefNbr, 1);
                if (addresses != null && addresses.Items != null && addresses.Items.Any())
                {
                    CustomerVm.Addresses = addresses.Items.Select(x => x.ToVm()).ToList();
                    CustomerVm.AddressesMetaData = addresses.MetaData;
                }
            }

        }
    }
}