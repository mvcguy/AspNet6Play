using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayWebApp.Services.Database.Model;

[Table("Tenant")]
public class Tenant
{
    [Key]
    [MaxLength(128)]
    [Column(TypeName = "nvarchar")]
    public string Id { get; set; }

    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(10)]
    public string TenantCode { get; set; }

    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public string TenantName { get; set; }

    [Required, Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public string Country { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? CreatedOn { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? ModifiedOn { get; set; }
    [Timestamp]
    public virtual byte[] Timestamp { get; set; }
}