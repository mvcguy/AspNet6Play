#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Identity;
using PlayWebApp.Services.Logistics.ViewModels;

namespace PlayWebApp.Areas.Identity.Pages.Account.Manage
{
    public class AddressModel : PageModel
    {
        private readonly UserManagementService userMgtSrv;

        public AddressModel(UserManagementService userMgtSrv)
        {
            this.userMgtSrv = userMgtSrv;
        }
        
        [TempData]
        public string StatusMessage { get; set; }

        public AddressUpdateVm AddressVm { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var defaultAddress = await userMgtSrv.GetUserDefaultAddress(userId);
            if (defaultAddress != null)
            {

                AddressVm = new AddressUpdateVm
                {
                    AddressCode = defaultAddress.AddressCode,
                    StreetAddress = defaultAddress.StreetAddress,
                    City = defaultAddress.City,
                    PostalCode = defaultAddress.PostalCode,
                    Country = defaultAddress.Country,
                    PreferredAddress = true
                };
            }
        }

    }
}
