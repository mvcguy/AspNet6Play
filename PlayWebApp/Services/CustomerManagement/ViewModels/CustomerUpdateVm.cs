using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
using PlayWebApp.Services.Logistics.ViewModels.Requests;

namespace PlayWebApp.Services.CustomerManagement.ViewModels
{
    public class CustomerUpdateVm : ViewModelBase
    {

        [Required]
        [Display(Name = "Reference")]
        [JsonPropertyName("CustomerVm.RefNbr")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public new string RefNbr { get; set; }

        [Required]
        [Display(Name = "Name")]
        [JsonPropertyName("CustomerVm.Name")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]        
        public string Name { get; set; }

        public bool Active { get; set; }

        public List<AddressUpdateVm> Addresses { get; set; }
    }

    public class CustomerDto : BaseDto
    {
        public string Name { get; set; }

        public bool Active { get; set; }

        public List<AddressDto> Addresses { get; set; }
    }

    public class CustomerRequestDto : RequestBase
    {
        
    }


}