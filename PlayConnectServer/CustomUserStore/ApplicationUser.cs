using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayConnectServer.CustomUserStore
{
    [Table("User")]
    public class ApplicationUser
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

        [Required, Column(TypeName = "nvarchar")]
        [MaxLength(128)]
        public virtual string FirstName { get; set; }

        [Required, Column(TypeName = "nvarchar")]
        [MaxLength(128)]
        public virtual string LastName { get; set; }

        [Required, Column(TypeName = "nvarchar")]
        [MaxLength(128)]
        public virtual string Email { get; set; }

        [Required, Column(TypeName = "nvarchar")]
        [MaxLength(128)]
        public virtual string UserName { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual DateTime? CreatedOn { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public virtual DateTime? ModifiedOn { get; set; }

        [Column(TypeName = "nvarchar")]
        [MaxLength(128)]
        public virtual string ModifiedBy { get; set; }

        [Timestamp]
        public virtual byte[] Timestamp { get; set; }

        [Column(TypeName = "nvarchar")]
        [MaxLength(128)]
        public virtual string CreatedBy { get; set; }
    }

}