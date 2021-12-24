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
