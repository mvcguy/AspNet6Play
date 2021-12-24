using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayWebApp.Services.Database.Model;

[Table("Tenant")]
public class Tenant
{
    [Key]
    [MaxLength(128)]
    [Column(TypeName = "nvarchar")]
    public virtual string Id { get; set; }

    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(10)]
    public virtual string TenantCode { get; set; }

    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string TenantName { get; set; }

    [Required, Column(TypeName = "char")]
    [MaxLength(2)]
    public virtual string Country { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual DateTime? CreatedOn { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public virtual DateTime? ModifiedOn { get; set; }

    [Timestamp]
    public virtual byte[] Timestamp { get; set; }
}