using PlayWebApp.Services.AppManagement.Repository;
using PlayWebApp.Services.AppManagement.ViewModels;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.ModelExtentions;
#nullable disable

namespace PlayWebApp.Services.AppManagement
{
    public class TenantService : NavigationService<Tenant, TenantRequestDto, TenantUpdateVm, TenantDto>
    {
        public TenantService(INavigationRepository<Tenant> repository) : base(repository)
        {
        }

        public override async Task<TenantDto> Add(TenantUpdateVm model)
        {
            var item = await repository.GetById(model.RefNbr);
            if (item != null) throw new Exception("Item exist from before");

            item = new Tenant
            {
                RefNbr = model.RefNbr,
                TenantName = model.Name,
                Country = model.Country
            };
            var record = repository.Add(item);

            return record.Entity.ToDto();
        }

        public override TenantDto ToDto(Tenant model)
        {
            return model.ToDto();
        }

        public override async Task<TenantDto> Update(TenantUpdateVm model)
        {
            var item = await repository.GetById(model.RefNbr);
            if (item == null) throw new Exception("Record does not exist");

            item.TenantName = model.Name;
            item.Country = model.Country;
            var record = repository.Update(item);

            return record.Entity.ToDto();
        }
    }
}