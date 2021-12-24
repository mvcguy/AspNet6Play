using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PlayWebApp.Services.Database.Model;

public abstract class Address : EntityBase
{
    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string StreetAddress { get; set; }

    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string PostalCode { get; set; }

    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string City { get; set; }

    [Required, Column(TypeName = "char")]
    [MaxLength(2)]
    public virtual string Country { get; set; }


}

public class CustomerAddress : Address
{
    public virtual string CustomerId { get; set; }

    public virtual Customer Customer { get; set; }
}

public class SupplierAddress : Address
{
    public virtual string SupplierId { get; set; }

    public virtual Supplier Supplier { get; set; }
}
