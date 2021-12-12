namespace PlayWebApp.Services.Database.Model;

public class IdentityUserExt : EntityBase
{
    public virtual string? FirstName { get; set; }

    public virtual string? LastName { get; set; }

    public virtual string? DefaultAddressId { get; set; }

    public new string? Code { get; }

}