namespace PlayWebApp.Services.Database.Model;

public class StockItem : EntityBase
{
    public virtual string Description { get; set; }

    public virtual ICollection<StockItemPrice> StockItemPrices { get; set; } = null!;

    public virtual ICollection<BookingItem> BookingItems { get; set; } = null!;

}
