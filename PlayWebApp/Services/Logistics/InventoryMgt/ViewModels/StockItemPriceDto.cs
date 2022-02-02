#nullable disable

using PlayWebApp;
using PlayWebApp.Services.Logistics.ViewModels;

namespace PlayWebApp.Services.Logistics.InventoryMgt.ViewModels
{
    public class StockItemPriceDto : BaseDto
    {
        public new virtual string RefNbr { get; set; }

        public virtual decimal BreakQty { get; set; }

        public virtual decimal UnitCost { get; set; }

        public virtual string UnitOfMeasure { get; set; }

        public virtual DateTime EffectiveFrom { get; set; }

        public virtual DateTime ExpiresAt { get; set; }
    }

}