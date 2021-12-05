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
