using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayWebApp.Services.Database.Model;

public class BookingItem : EntityBase
{
    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string StockItemId { get; set; }

    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string Description { get; set; }

    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string BookingId { get; set; }

    [Required, Column(TypeName = "nvarchar")]
    public virtual decimal? Quantity { get; set; }

    [Column(TypeName = "decimal(2,4)")]
    public virtual decimal? UnitCost { get; set; }

    [Required, Column(TypeName = "decimal(2,4)")]
    public virtual decimal? ExtCost { get; set; }

    [Column(TypeName = "decimal(2,4)")]
    public virtual decimal? Discount { get; set; }

    public virtual Booking Booking { get; set; }

    public virtual StockItem StockItem { get; set; }
}
