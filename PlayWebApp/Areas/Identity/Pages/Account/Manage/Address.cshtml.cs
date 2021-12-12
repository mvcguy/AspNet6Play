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
using PlayWebApp.Services.Logistics.ViewModels;

namespace PlayWebApp.Areas.Identity.Pages.Account.Manage
{
    public class AddressModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public AddressModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public AddressUpdateVm AddressVm { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var first = await dbContext.Addresses.OrderBy(x => x.Code).FirstOrDefaultAsync(x => x.UserId == userId);
            if (first != null)
            {

                AddressVm = new AddressUpdateVm
                {
                    AddressCode = first.Code,
                    StreetAddress = first.StreetAddress,
                    City = first.City,
                    PostalCode = first.PostalCode,
                    Country = first.Country,
                    PreferredAddress = await IsPreferredAddress(userId, first.Id)
                };
            }
        }

        private async Task<bool> IsPreferredAddress(string userId, string addressId)
        {
            var userExt = await dbContext.Set<IdentityUserExt>().FirstOrDefaultAsync(x => x.UserId == userId && x.DefaultAddressId == addressId);
            if(userExt!=null) return true;

            return false;
        }
    }
}
