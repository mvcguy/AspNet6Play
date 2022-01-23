#nullable disable
using System.ComponentModel.DataAnnotations;
using PlayWebApp;
using PlayWebApp.Services.Logistics.ViewModels;

namespace PlayWebApp.Services.Logistics.BookingMgt.ViewModels
{
    public class BookingItemUpdateVm : ViewModelBase
    {
        
        [Required]
        [Display(Name = "Line Nbr")]   
        [StringLength(3, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]     
        public new string RefNbr { get; set; }

        [Required]
        [Display(Name = "Description")]   
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]     
        public string Description { get; set; }

        [Display(Name = "Discount")] 
        public decimal? Discount { get; set; }

        [Required]
        [Display(Name = "Cost")] 
        public decimal? ExtCost { get; set; }

        [Required]
        [Display(Name = "Quantity")] 
        public decimal? Quantity { get; set; }

        [Required]
        [Display(Name = "Stock item")] 
        public string StockItemRefNbr { get; set; }

        [Display(Name = "Unit cost")] 
        public decimal? UnitCost { get; set; }
    }
}