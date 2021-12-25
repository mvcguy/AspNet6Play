#nullable disable

using System.ComponentModel.DataAnnotations;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;

namespace PlayWebApp.Services.Identity.ViewModels
{
    public class ApplicationUserDto
    {

        public virtual string Key { get; set; }

        public virtual string TenantId { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }
    }

    public class ApplicationUserUpdateVm
    {

        public virtual string UserId { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Email{ get; set; }

        [Required]
        public virtual string TenantId { get; set; }

        [Required]
        [Display(Name = "First name")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public virtual string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public virtual string LastName { get; set; }

    }
}