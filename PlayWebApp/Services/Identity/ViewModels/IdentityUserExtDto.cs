#nullable disable

using System.ComponentModel.DataAnnotations;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;

namespace PlayWebApp.Services.Identity.ViewModels
{
    public class IdentityUserExtDto
    {

        public virtual string Key { get; set; }

        public virtual string TenantId { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string DefaultAddressId { get; set; }

        public virtual AddressDto DefaultAddress { get; set; }
    }

    public class IdentityUserUpdateVm
    {

        public virtual string UserId { get; set; }

        [Required]
        public virtual string TenantCode { get; set; }

        [Required]
        [Display(Name = "First name")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public virtual string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public virtual string LastName { get; set; }

        public virtual AddressUpdateVm DefaultAddress { get; set; }

    }
}