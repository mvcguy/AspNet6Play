using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PlayWebApp.Services.JsonConverters;
#nullable disable
namespace PlayWebApp.Services.Logistics.ViewModels
{
    public class AddressUpdateVm : ViewModelBase
    {

        [Required]
        [Display(Name = "Reference")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public new string RefNbr { get; set; }

        [Required]
        [Display(Name = "Street address:")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string StreetAddress { get; set; }

        [Required]
        [Display(Name = "City:")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string City { get; set; }

        [Required]
        [Display(Name = "Postal code:")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name = "Country:")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Country { get; set; }

        public UpdateType UpdateType { get; set; }
        
        [Required]
        [Range(0, 20000)]        
        public int ClientRowNumber { get; set; }

    }

    public enum UpdateType
    {
        None = 0,
        Update = 1,
        Delete = 2,
        New = 3
    }

}