using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
using PlayWebApp.Services.Logistics.ViewModels.Requests;
using PlayWebApp.Services.ModelExtentions;
#nullable disable

namespace PlayWebApp.Services.Logistics.LocationMgt
{
    public class LocationService : NavigationService<Address, AddressRequestDto, AddressUpdateVm, AddressDto>
    {
        public LocationService(INavigationRepository<Address> repository) : base(repository)
        {
        }

        public async override Task<AddressDto> Add(AddressUpdateVm model)
        {
            var record = await repository.GetById(model.AddressCode);
            if (record == null)
            {
                record = new Address
                {
                    Country = model.Country,
                    City = model.City,
                    StreetAddress = model.StreetAddress,
                    PostalCode = model.PostalCode,
                    Code = model.AddressCode,
                };
                var item = repository.Add(record);
                return item.Entity.ToDto();
            }

            throw new Exception("Record exist from before");
        }

        public override AddressDto ToDto(Address model)
        {
            return model.ToDto();
        }

        public async override Task<AddressDto> Update(AddressUpdateVm model)
        {
            var record = await repository.GetById(model.AddressCode);
            if (record != null)
            {
                record.Country = model.Country;
                record.City = model.City;
                record.StreetAddress = model.StreetAddress;
                record.PostalCode = model.PostalCode;

                var item = repository.Update(record);
                return item.Entity.ToDto();
            }

            throw new Exception("Record cannot be found");
        }
    }
}