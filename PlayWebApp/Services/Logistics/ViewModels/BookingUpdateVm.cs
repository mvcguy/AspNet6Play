using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
#nullable disable
namespace PlayWebApp.Services.Logistics.ViewModels
{
    public class BookingUpdateVm : ViewModelBase
    {

        [Required]
        [Display(Name = "Booking number:")]
        [JsonPropertyName("BookingVm.RefNbr")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public new string RefNbr { get; set; }

        [Required]
        [Display(Name = "Description")]
        [JsonPropertyName("BookingVm.Description")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Customer")]
        [JsonPropertyName("BookingVm.CustomerRefNbr")]
        public string CustomerRefNbr { get; set; }

        public IList<BookingItemUpdateVm> Lines { get; set; }
    }
}