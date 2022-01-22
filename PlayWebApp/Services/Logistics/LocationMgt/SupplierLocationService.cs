using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.Logistics.LocationMgt.ViewModels;
using PlayWebApp.Services.ModelExtentions;
#nullable disable

namespace PlayWebApp.Services.Logistics.LocationMgt
{

    public class SupplierLocationService : NavigationService<SupplierAddress, AddressRequestDto, AddressUpdateVm, AddressDto>
    {
        public SupplierLocationService(INavigationRepository<SupplierAddress> repository) : base(repository)
        {
        }

        public async override Task<AddressDto> Add(AddressUpdateVm model)
        {
            var record = await repository.GetById(model.RefNbr);
            if (record == null)
            {
                record = new SupplierAddress
                {
                    Country = model.Country,
                    City = model.City,
                    StreetAddress = model.StreetAddress,
                    PostalCode = model.PostalCode,
                    RefNbr = model.RefNbr,
                };
                var item = repository.Add(record);
                return item.Entity.ToDto();
            }

            throw new Exception("Record exist from before");
        }

        public override AddressDto ToDto(SupplierAddress model)
        {
            return model.ToDto();
        }

        public async override Task<AddressDto> Update(AddressUpdateVm model)
        {
            var record = await repository.GetById(model.RefNbr);
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