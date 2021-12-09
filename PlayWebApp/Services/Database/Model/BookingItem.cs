namespace PlayWebApp.Services.Database.Model;

public class BookingItem : EntityBase
{
    public virtual string? StockItemId { get; set; }

    public virtual string? Description { get; set; }

    public virtual string? BookingId { get; set; }

    public virtual decimal? Quantity { get; set; } = null!;

    public virtual decimal? UnitCost { get; set; } = null!;

    public virtual decimal? ExtCost { get; set; } = null!;

    public virtual decimal? Discount { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;

    public virtual StockItem StockItem { get; set; } = null!;
}
