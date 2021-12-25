using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayWebApp.Services.Database.Model;

[Table("Tenant")]
public class Tenant : EntityBase
{   
    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string TenantName { get; set; }

    [Required, Column(TypeName = "char")]
    [MaxLength(2)]
    public virtual string Country { get; set; }  
    
    public new string TenantId { get; }
}