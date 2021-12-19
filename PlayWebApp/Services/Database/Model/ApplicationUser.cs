namespace PlayWebApp.Services.Database.Model;

public class ApplicationUser : EntityBase
{
    public virtual string FirstName { get; set; }

    public virtual string LastName { get; set; }

    public virtual string DefaultAddressId { get; set; }

    public new string Code { get; }
    public string Email { get; set; }
    public string UserName { get; set; }
}