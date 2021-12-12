using PlayWebApp.Services.AppManagement.Repository;
using PlayWebApp.Services.AppManagement.ViewModels;
using PlayWebApp.Services.ModelExtentions;
#nullable disable

namespace PlayWebApp.Services.AppManagement
{
    public class AppMgtService
    {
        private readonly AppMgtRepository repository;

        public AppMgtService(AppMgtRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<TenantDto>> GetAllTenants(int top = 100, int skip = 0, string customerId = null)
        {
            return (await repository.GetTenants(top, skip, customerId)).Select(x=>x.ToDto());
        }

        public async Task<TenantDto> GetTenantByCode(string code)
        {
            return (await repository.GetTenantByCode(code)).ToDto();
        }
    }
}