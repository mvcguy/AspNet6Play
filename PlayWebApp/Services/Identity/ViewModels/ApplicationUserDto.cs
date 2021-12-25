
using System.ComponentModel.DataAnnotations;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
using PlayWebApp.Services.Logistics.ViewModels.Requests;

namespace PlayWebApp.Services.Identity.ViewModels
{
    public class AppUserDto : BaseDto
    {
        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }
    }

    public class AppUserUpdateVm : ViewModelBase
    {

        public virtual string UserName { get; set; }

        public virtual string Email { get; set; }

        [Required]
        [Display(Name = "First name")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public virtual string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public virtual string LastName { get; set; }

    }

    public class AppUserRequestDto : RequestBase
    {

    }
}