using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayWebApp.Services.Database.Model;

public class StockItemPrice : EntityBase
{
    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string StockItemId { get; set; }

    [Required, Column(TypeName = "decimal(2, 4)")]
    public virtual decimal BreakQty { get; set; }

    [Required, Column(TypeName = "decimal(2, 4)")]
    public virtual decimal UnitCost { get; set; }

    [Required, Column(TypeName = "char")]
    [MaxLength(10)]
    public virtual string UnitOfMeasure { get; set; }

    public virtual DateTime EffectiveFrom { get; set; }

    public virtual DateTime ExpiresAt { get; set; }

    public virtual StockItem StockItem { get; set; }
}
