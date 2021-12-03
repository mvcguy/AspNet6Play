using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
#nullable disable
namespace PlayWebApp.Services.ViewModels
{
    public class StockItemUpdateVm
    {
        [Required]
        [Display(Name = "Item ID:")]
        [JsonPropertyName("StockItemVm.ItemDisplayId")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string ItemDisplayId { get; set; }

        [Required]
        [Display(Name = "Description")]
        [JsonPropertyName("StockItemVm.ItemDescription")]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string ItemDescription { get; set; }


    }

    public class StockItemDto
    {
       
        public string ItemDisplayId { get; set; }
                
        public string ItemDescription { get; set; }
    }

}