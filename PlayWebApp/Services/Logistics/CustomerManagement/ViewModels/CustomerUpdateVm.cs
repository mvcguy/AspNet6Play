using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PlayWebApp.Services.Logistics.LocationMgt.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels;

namespace PlayWebApp.Services.Logistics.CustomerManagement.ViewModels
{
    public class CustomerUpdateVm : ViewModelBase
    {

        [Required]
        [Display(Name = "Reference")]
        [JsonProperty("CustomerVm.RefNbr")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public new string RefNbr { get; set; }

        [Required]
        [Display(Name = "Name")]
        [JsonProperty("CustomerVm.Name")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Name { get; set; }

        public bool Active { get; set; }

        public List<AddressUpdateVm> Addresses { get; set; }

        public CollectionMetaData AddressesMetaData { get; set; }
    }

    public class CustomerRequestDto : RequestBase
    {

    }


}