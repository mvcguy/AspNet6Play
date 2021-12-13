using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
using PlayWebApp.Services.Logistics.ViewModels.Requests;
using PlayWebApp.Services.ModelExtentions;

namespace PlayWebApp.Services.Logistics.LocationMgt
{
    public class LocationService : NavigationService<Address, AddressRequestDto, AddressUpdateVm, AddressDto>
    {
        public LocationService(INavigationRepository<Address> repository) : base(repository)
        {
        }

        public override Address ToDbModel(AddressUpdateVm vm)
        {
            return vm.ToModel();
        }

        public override AddressDto ToDto(Address model)
        {
            return model.ToDto();
        }
    }
}