using Microsoft.AspNetCore.Identity;

namespace PlayWebApp.Services.Database.Model;

public class Booking : EntityBase
{
    public virtual string? Description { get; set; }

    public virtual DateTime BookingDate { get; set; }
    public virtual string? ShippingAddressId { get; set; }

    public virtual ICollection<BookingItem> BookingItems { get; set; } = null!;

    public virtual Address ShippingAddress { get; set; } = null!;

}
