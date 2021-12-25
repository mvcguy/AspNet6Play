using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayWebApp.Services.Database.Model
{
    
    public abstract class BusinessEntity : EntityBase
    {
        [Required]
        [StringLength(128)]
        [Column(TypeName = "nvarchar")]
        public virtual string Name { get; set; }

        [Column(TypeName = "bit")]
        public virtual bool Active { get; set; }

    }

    public class Customer : BusinessEntity
    {        
        public virtual ICollection<Booking> Bookings { get; set; }

        public virtual ICollection<CustomerAddress> Addresses { get; set; }

        public string PaymentMethod { get; set; }

    }

    public class Supplier : BusinessEntity
    {
        public virtual ICollection<SupplierAddress> Addresses { get; set; }

        public string ExpenseAccount { get; set; }
    }

}