using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
#nullable disable
namespace PlayWebApp.Services.ViewModels
{
    public class BookingUpdateVm
    {

        [Required]
        [Display(Name = "Booking number:")]
        [JsonPropertyName("BookingVm.BookingNumber")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string BookingNumber { get; set; }

        [Required]
        [Display(Name = "Description")]
        [JsonPropertyName("BookingVm.Description")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Description { get; set; }
    }

}