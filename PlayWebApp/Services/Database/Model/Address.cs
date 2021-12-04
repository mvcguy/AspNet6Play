namespace PlayWebApp.Services.Database.Model;

public class Address
{
    public virtual Guid? Id { get; set; }

    public virtual string? StreetAddress { get; set; }

    public virtual string? PostalCode { get; set; }

    public virtual string? City { get; set; }

    public virtual string? Country { get; set; }

    public virtual string? UserId { get; set; }

    public virtual IdentityUserExt UserExt { get; set; } = null!;

}
