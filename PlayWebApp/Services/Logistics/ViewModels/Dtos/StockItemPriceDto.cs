#nullable disable

using PlayWebApp;

namespace PlayWebApp.Services.Logistics.ViewModels.Dtos
{
    public class StockItemPriceDto : BaseDto
    {
        public virtual string StockItemId { get; set; }

        public virtual decimal BreakQty { get; set; }

        public virtual decimal UnitCost { get; set; }

        public virtual string UnitOfMeasure { get; set; }

        public virtual DateTime EffectiveFrom { get; set; }

        public virtual DateTime ExpiresAt { get; set; }
    }

}