namespace PlayWebApp.Services.Database.Model;

public class StockItem
{
    public virtual Guid? Id { get; set; }

    public virtual string? DisplayId { get; set; }

    public virtual string? Description { get; set; }

    public virtual ICollection<StockItemPrice> StockItemPrices { get; set; } = null!;

    public virtual ICollection<BookingItem> BookingItems { get; set; } = null!;

}
