using System.ComponentModel.DataAnnotations;
using PlayWebApp.Services.Logistics.ViewModels;
#nullable disable

namespace PlayWebApp.Services.AppManagement.ViewModels
{
    public class TenantUpdateVm : ViewModelBase
    {
        [Required]
        [Display(Name = "Tenant code")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public new string RefNbr { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Country")]
        [StringLength(2, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Country { get; set; }
    }

    public class TenantDto : BaseDto
    {
        public string Name { get; set; }

        public string Country { get; set; }
    }

    public class TenantRequestDto : RequestBase
    {
        
    }
}