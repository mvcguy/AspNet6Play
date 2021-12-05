using Microsoft.AspNetCore.Identity;

namespace PlayWebApp.Services.Database.Model;

public class IdentityUserExt
{
    public virtual string? UserId { get; set; }

    public virtual string? FirstName { get; set; }

    public virtual string? LastName { get; set; }

    public virtual Guid? DefaultAddressId { get; set; }

    public virtual IdentityUser User { get; set; } = null!;

    public virtual ICollection<Address> Addresses { get; set; } = null!;
}