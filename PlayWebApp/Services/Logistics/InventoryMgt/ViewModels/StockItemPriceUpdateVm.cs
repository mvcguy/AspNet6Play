using System.ComponentModel.DataAnnotations;
using PlayWebApp.Services.Logistics.ViewModels;
#nullable disable
namespace PlayWebApp.Services.Logistics.InventoryMgt.ViewModels
{
    public class StockItemPriceUpdateVm : ViewModelBase
    {
        [Required]
        [Display(Name = "Line Nbr")]
        public new virtual string RefNbr { get; set; }

        [Display(Name = "Breaking Qty")]
        public virtual decimal BreakQty { get; set; }

        [Display(Name = "Unit cost")]
        public virtual decimal UnitCost { get; set; }

        [Display(Name = "UOM")]
        public virtual string UnitOfMeasure { get; set; }

        [Display(Name ="Effective From")]
        public virtual DateTime EffectiveFrom { get; set; }

        [Display(Name ="Expires On")]
        public virtual DateTime ExpiresAt { get; set; }
    }

}