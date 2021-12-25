using PlayWebApp.Services.AppManagement.Repository;
using PlayWebApp.Services.AppManagement.ViewModels;
using PlayWebApp.Services.Database.Model;
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
            return (await repository.GetTenants(top, skip, customerId)).Select(x => x.ToDto());
        }

        public async Task<TenantDto> GetTenantByCode(string code)
        {
            return (await repository.GetTenantByCode(code)).ToDto();
        }

        public async Task<TenantDto> GetTenantById(string id)
        {
            return (await repository.GetTenantById(id)).ToDto();
        }

        public string CreateTenant(TenantUpdateVm model)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid().ToString(),
                Country = model.Country,
                TenantCode = model.RefNbr,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                TenantName = model.Name
            };
            var result = repository.CreateTenant(tenant);

            if (!result.Succeeded) throw new Exception("Could not create tenant.");

            return result.EntityId;
        }
    }
}