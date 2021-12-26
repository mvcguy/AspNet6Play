using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.CustomerManagement;
using PlayWebApp.Services.CustomerManagement.ViewModels;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Logistics.ViewModels;
using System.Linq;
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

        public async Task OnGet()
        {
            var top1 = await customerService.GetFirst();
            if (top1 != null)
            {
                CustomerVm = new CustomerUpdateVm { RefNbr = top1.RefNbr, Name = top1.Name, Active = top1.Active };
            }

        }
    }
}