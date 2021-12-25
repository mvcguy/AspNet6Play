using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
#nullable disable
namespace PlayWebApp.Services.Logistics.ViewModels
{
    public class StockItemUpdateVm : ViewModelBase
    {
        [Required]
        [Display(Name = "Item ID:")]
        [JsonPropertyName("StockItemVm.ItemDisplayId")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public new string RefNbr { get; set; }

        [Required]
        [Display(Name = "Description")]
        [JsonPropertyName("StockItemVm.ItemDescription")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string ItemDescription { get; set; }

        public IList<StockItemPriceUpdateVm> ItemPrices { get; set; }
    }

}