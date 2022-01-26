using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayWebApp.Services.Database.Model;

public class Booking : EntityBase
{
    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string Description { get; set; }

    public virtual DateTime BookingDate { get; set; }

    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string ShippingAddressId { get; set; }

    public virtual string CustomerId { get; set; }

    public virtual Customer Customer { get; set; }

    public virtual ICollection<BookingItem> BookingItems { get; set; }

    public virtual Address ShippingAddress { get; set; }

    #region Summary

    [Column(TypeName = "decimal(2,4)")]
    public virtual decimal LinesTotal { get; set; }

    [Column(TypeName = "decimal(2,4)")]
    public virtual decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(2,4)")]
    public virtual decimal Balance { get; set; }

    [Column(TypeName = "decimal(2,4)")]
    public virtual decimal Discount { get; set; }

    [Column(TypeName = "decimal(2,4)")]
    public virtual decimal TaxableAmount { get; set; }

    [Column(TypeName = "decimal(2,4)")]
    public virtual decimal TaxAmount { get; set; }

    #endregion Summary

}
