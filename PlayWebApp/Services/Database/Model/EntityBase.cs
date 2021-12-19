using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
#nullable disable

namespace PlayWebApp.Services.Database.Model;


public interface IAuditInfo
{
    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string CreatedBy { get; set; }

    public string ModifiedBy { get; set; }

    public byte[] Timestamp { get; set; }
}

public class AuditInfo : IAuditInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? CreatedOn { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? ModifiedOn { get; set; }

    [ForeignKey("ApplicationUser")]
    public string ModifiedBy { get; set; }

    [Timestamp]
    public virtual byte[] Timestamp { get; set; }

    [Required]
    [Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string CreatedBy { get; set; }
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