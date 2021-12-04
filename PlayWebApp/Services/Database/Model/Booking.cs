using Microsoft.AspNetCore.Identity;

namespace PlayWebApp.Services.Database.Model;

public class Booking
{
    public virtual Guid? Id { get; set; }

    public virtual string? BookingNumber { get; set; }

    public virtual string? Description { get; set; }

    public virtual DateTime BookingDate { get; set; }

    public virtual string? UserId { get; set; }

    public virtual Guid? ShippingAddressId { get; set; }

    public virtual IdentityUser User { get; set; } = null!;

    public virtual ICollection<BookingItem> BookingItems { get; set; } = null!;

    public virtual Address ShippingAddress { get; set; } = null!;

}

public class BookingItem
{
    public virtual Guid? StockItemId { get; set; }

    public virtual string? Description { get; set; }

    public virtual Guid? BookingId { get; set; }

    public virtual decimal? Quantity { get; set; } = null!;

    public virtual decimal? UnitCost { get; set; } = null!;

    public virtual decimal? ExtCost { get; set; } = null!;

    public virtual decimal? Discount { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;

    public virtual StockItem StockItem { get; set; } = null!;
}

public class StockItem
{
    public virtual Guid? Id { get; set; }

    public virtual string? DisplayId { get; set; }

    public virtual string? Description { get; set; }

    public virtual ICollection<StockItemPrice> StockItemPrices { get; set; } = null!;

    public virtual ICollection<BookingItem> BookingItems { get; set; } = null!;

}

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

public class Address
{
    public virtual Guid? Id { get; set; }

    public virtual string? StreetAddress { get; set; }

    public virtual string? PostalCode { get; set; }

    public virtual string? City { get; set; }

    public virtual string? Country { get; set; }
}
