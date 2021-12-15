#nullable disable
namespace PlayWebApp.Services.AppManagement
{
    public interface IPlayAppContext
    {
        string UserId { get; }

        string TenantId { get; }
    }

}