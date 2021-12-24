using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
#nullable disable

namespace PlayWebApp.Services.Database.Model;

public abstract class EntityBase : AuditInfo
{

    [Key]
    [Required]
    [MaxLength(128)]
    [Column(TypeName = "nvarchar")]
    public virtual string Id { get; set; }

    [Required]
    [MaxLength(10)]
    [Column(TypeName = "nvarchar")]
    public virtual string RefNbr { get; set; }

    [Required]
    [MaxLength(128)]
    [Column(TypeName = "nvarchar")]
    public virtual string TenantId { get; set; }

}