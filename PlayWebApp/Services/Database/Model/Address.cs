using Microsoft.AspNetCore.Identity;

namespace PlayWebApp.Services.Database.Model;

public class Address : EntityBase
{
    public virtual string? StreetAddress { get; set; }

    public virtual string? PostalCode { get; set; }

    public virtual string? City { get; set; }

    public virtual string? Country { get; set; }
}
