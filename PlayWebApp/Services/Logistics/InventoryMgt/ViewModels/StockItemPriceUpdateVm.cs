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

        [Required, Display(Name = "Breaking Qty")]
        public virtual decimal? BreakQty { get; set; }

        [Required, Display(Name = "Unit cost")]
        public virtual decimal? UnitCost { get; set; }

        [Required, Display(Name = "UOM")]
        public virtual string UnitOfMeasure { get; set; }

        [Required, Display(Name ="Effective From")]
        public virtual DateTime? EffectiveFrom { get; set; }

        [Required, Display(Name ="Expires On")]
        public virtual DateTime? ExpiresAt { get; set; }
    }

}