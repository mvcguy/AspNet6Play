namespace PlayWebApp.Services.Database.Model;

public class StockItemPrice
{

    public virtual Guid? Id { get; set; }
    public virtual Guid? StockItemId { get; set; }

    public virtual decimal BreakQty { get; set; }

    public virtual decimal UnitCost { get; set; }

    public virtual string? UnitOfMeasure { get; set; }

    public virtual DateTime EffectiveFrom { get; set; }

    public virtual DateTime ExpiresAt { get; set; }

    public virtual StockItem StockItem { get; set; } = null!;
}
