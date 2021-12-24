using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace PlayWebApp.Services.Database.Model;

public abstract class AuditInfo : IAuditInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual DateTime? CreatedOn { get; set; }


    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public virtual DateTime? ModifiedOn { get; set; }

    [Required]
    [Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string ModifiedBy { get; set; }

    [Timestamp]
    public virtual byte[] Timestamp { get; set; }

    [Required]
    [Column(TypeName = "nvarchar")]
    [MaxLength(128)]
    public virtual string CreatedBy { get; set; }
}
