// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Logistics.ViewModels;

namespace PlayWebApp.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManagerExt _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(
            UserManagerExt userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>

        public IEnumerable<SelectListItem> UserAddressesList { get; set; }

        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }


            [Required]
            [Display(Name = "First name")]
            [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last name")]
            [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            public string LastName { get; set; }

            public AddressDto AddressVm { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            var userExt = await _userManager.GetUserExtAsync(user.Id);
            var addresses = await _userManager.GetUserAddressesAsync(user.Id);
            var address = addresses.FirstOrDefault(x => x.Id == userExt.DefaultAddressId) ?? new Address();
            UserAddressesList = addresses.Select(x => new SelectListItem
            {
                Value = x.Code,
                Text = $"{x.Code} - {x.StreetAddress}"
            });
            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = userExt?.FirstName,
                LastName = userExt?.LastName,
                AddressVm = new AddressDto
                {
                    AddressCode = address.Code,
                    City = address.City,
                    Country = address.Country,
                    PostalCode = address.PostalCode,
                    PreferredAddress = true,
                    StreetAddress = address.StreetAddress
                }
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            //
            // save first name and last name
            //
            var userExt = await _userManager.GetUserExtAsync(user.Id);
            await UpdateAddress(user, userExt);

            var result = await _userManager.UpdateExtendedUser(userExt);
            if (!result.Succeeded)
            {
                StatusMessage = $"Error updating profile. Error: {result.Errors.FirstOrDefault()}";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        private async Task UpdateAddress(IdentityUser user, IdentityUserExt userExt)
        {
            
            userExt.FirstName = Input.FirstName;
            userExt.LastName = Input.LastName;

            // selected address is different than existing one, update the default address
            var currentAddress = await _userManager.GetUserDefaultAddress(userExt.UserId, userExt.Code);
            if (!string.IsNullOrWhiteSpace(Input.AddressVm.AddressCode) && currentAddress?.Code != Input.AddressVm.AddressCode)
            {
                var newAddress = await _userManager.GetUserAddress(userExt.UserId, Input.AddressVm.AddressCode);
                if (newAddress != null)
                {
                    userExt.DefaultAddressId = newAddress.Id;
                }
            }
        }
    }
}
