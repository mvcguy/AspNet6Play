using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
#nullable disable

namespace PlayWebApp.Services.Database.Model;


public interface IAuditInfo
{
    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string UserId { get; set; }

    public string ModifiedBy { get; set; }

    public byte[] Timestamp { get; set; }
}

public class AuditInfo : IAuditInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? CreatedOn { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? ModifiedOn { get; set; }

    [ForeignKey("IdentityUser")]
    public string ModifiedBy { get; set; }

    [Timestamp]
    public virtual byte[] Timestamp { get; set; }

    [Required]
    [Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string UserId { get; set; }

    public virtual IdentityUser User { get; set; } = null!;
}

public class EntityBase : AuditInfo
{

    [Key]
    public virtual string Id { get; set; }

    [Required]
    [MaxLength(10)]
    [Column(TypeName = "nvarchar")]
    public virtual string Code { get; set; }

    [Required]
    public virtual string TenantId { get; set; }

}
