using System.ComponentModel.DataAnnotations;
#nullable disable

namespace PlayWebApp.Services.AppManagement.ViewModels
{
    public class TenantUpdateVm
    {
        [Required]
        [Display(Name = "Tenant code")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Country")]
        [StringLength(2, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Country { get; set; }
    }

    public class TenantDto
    {       

        public string Key { get; set; }

        public string Code { get; set; }
        
        public string Name { get; set; }
       
        public string Country { get; set; }
    }
}