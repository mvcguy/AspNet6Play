using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
#nullable disable
namespace PlayWebApp.Services.Logistics.BookingMgt.ViewModels
{
    public class BookingUpdateVm : ViewModelBase
    {

        [Required]
        [Display(Name = "Reference")]
        [JsonProperty("BookingVm.RefNbr")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public new string RefNbr { get; set; }

        [Required]
        [Display(Name = "Description")]
        [JsonProperty("BookingVm.Description")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Customer")]
        [JsonProperty("BookingVm.CustomerRefNbr")]
        public string CustomerRefNbr { get; set; }

        public virtual decimal LinesTotal { get; set; }

        public virtual decimal TotalAmount { get; set; }

        public virtual decimal Balance { get; set; }

        public virtual decimal Discount { get; set; }

        public virtual decimal TaxableAmount { get; set; }

        public virtual decimal TaxAmount { get; set; }

        public IList<BookingItemUpdateVm> Lines { get; set; }

        public CollectionMetaData LinesMetaData { get; set; }

        public BookingUpdateVm()
        {
            Lines = new List<BookingItemUpdateVm>();
        }
    }
}