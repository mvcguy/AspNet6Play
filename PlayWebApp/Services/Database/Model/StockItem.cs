using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayWebApp.Services.Database.Model;

public class StockItem : EntityBase
{

    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string Description { get; set; }

    public virtual ICollection<StockItemPrice> StockItemPrices { get; set; } 

    public virtual ICollection<BookingItem> BookingItems { get; set; } 

}
