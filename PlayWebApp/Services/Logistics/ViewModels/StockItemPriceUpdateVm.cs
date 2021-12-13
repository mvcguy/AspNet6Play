using System.ComponentModel.DataAnnotations;
#nullable disable
namespace PlayWebApp.Services.Logistics.ViewModels
{
    public class StockItemPriceUpdateVm : ViewModelBase
    {
        [Required]
        [Display(Name = "Item ID")]
        public virtual string Code { get; set; }

        public virtual decimal BreakQty { get; set; }

        public virtual decimal UnitCost { get; set; }

        public virtual string UnitOfMeasure { get; set; }

        public virtual DateTime EffectiveFrom { get; set; }

        public virtual DateTime ExpiresAt { get; set; }
    }

}