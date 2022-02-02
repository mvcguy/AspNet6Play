using System.Linq.Expressions;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.Logistics.LocationMgt.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.ModelExtentions;
#nullable disable

namespace PlayWebApp.Services.Logistics.CustomerManagement
{
    public class CustomerLocationService : NavigationService<CustomerAddress, AddressRequestDto, AddressUpdateVm, AddressDto>
    {
        private readonly INavigationRepository<Customer> customerRepo;

        public CustomerLocationService(INavigationRepository<Customer> customerRepo,
            INavigationRepository<CustomerAddress> repository) : base(repository)
        {
            this.customerRepo = customerRepo;
        }

        public async Task<DtoCollection<AddressDto>> GetAllByCustomerId(string customerRefNbr, int page = 1)
        {
            var customer = await customerRepo.GetById(customerRefNbr);

            var result = (await base.GetPaginatedCollection(x => x.Customer.RefNbr == customerRefNbr, page));

            // mark default address
            if (customer.DefaultAddress != null)
            {
                var defAdd = result.Items.FirstOrDefault(x => x.RefNbr == customer.DefaultAddress.RefNbr);
                if (defAdd != null)
                    defAdd.IsDefault = true;
            }

            return result;
        }

        public async override Task<AddressDto> Add(AddressUpdateVm model)
        {
            var record = await repository.GetById(model.RefNbr);
            if (record == null)
            {
                record = new CustomerAddress
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

        public override AddressDto ToDto(CustomerAddress model)
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