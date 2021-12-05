using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
#nullable disable
namespace PlayWebApp.Services.Logistics.ViewModels
{
    public class AddressUpdateVm
    {

        [Required]
        [Display(Name = "Address code")]
        [JsonPropertyName("AddressVm.AddressCode")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string AddressCode { get; set; }

        [Required]
        [Display(Name = "Street address:")]
        [JsonPropertyName("AddressVm.StreetAddress")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string StreetAddress { get; set; }

        [Required]
        [Display(Name = "City:")]
        [JsonPropertyName("AddressVm.City")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string City { get; set; }

        [Required]
        [Display(Name = "Postal code:")]
        [JsonPropertyName("AddressVm.PostalCode")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name = "Country:")]
        [JsonPropertyName("AddressVm.Country")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Country { get; set; }
    }

    public class AddressDto
    {
        public string AddressCode { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }
    }

}