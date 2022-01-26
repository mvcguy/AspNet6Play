using PlayWebApp.Services.Logistics.CustomerManagement.ViewModels;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.ModelExtentions;
using PlayWebApp.Services.Logistics.LocationMgt.ViewModels;

namespace PlayWebApp.Services.Logistics.CustomerManagement
{
    public class CustomerService : NavigationService<Customer, CustomerRequestDto, CustomerUpdateVm, CustomerDto>
    {
        private readonly INavigationRepository<CustomerAddress> locRepository;

        public CustomerService(INavigationRepository<Customer> repository,
                                 INavigationRepository<CustomerAddress> locRepository) : base(repository)
        {
            this.locRepository = locRepository;
        }

        public override async Task<CustomerDto> Add(CustomerUpdateVm model)
        {
            var item = await repository.GetById(model.RefNbr);
            if (item != null) throw new Exception("Record exist from before");

            item = new Customer
            {
                Name = model.Name,
                Active = model.Active,
                RefNbr = model.RefNbr,
                Addresses = new List<CustomerAddress>()
            };
            UpdateCustomerAddresses(model, item);
            var entry = repository.Add(item);
            return entry.Entity.ToDto();
        }

        public override CustomerDto ToDto(Customer model)
        {
            return model.ToDto();
        }

        public override async Task<CustomerDto> Update(CustomerUpdateVm model)
        {
            var item = await repository.GetById(model.RefNbr);
            if (item == null) throw new Exception("Record does not exist");

            item.Name = model.Name;
            item.Active = model.Active;

            item.Addresses = (await locRepository.GetCollection(x => x.Customer.RefNbr == model.RefNbr)).ToList();

            UpdateCustomerAddresses(model, item);

            var entry = repository.Update(item);

            return entry.Entity.ToDto();

        }

        private void UpdateCustomerAddresses(CustomerUpdateVm model, Customer customer)
        {
            model.Addresses = model.Addresses ?? new List<AddressUpdateVm>();
            foreach (var addressVm in model.Addresses)
            {
                CustomerAddress address = null;
                switch (addressVm.UpdateType)
                {
                    case UpdateType.New:
                        address = AddNewAddress(customer, addressVm);

                        break;
                    case UpdateType.Update:
                        address = UpdateExistingAddress(customer, addressVm);
                        break;
                    case UpdateType.Delete:
                        address = DeleteExistingAddress(customer, addressVm);
                        break;
                }

                if (address != null)
                {
                    customer.DefaultAddress = addressVm.IsDefault && addressVm.UpdateType != UpdateType.Delete
                                                ? address
                                                : null;
                }

            }
        }

        private CustomerAddress DeleteExistingAddress(Customer item, AddressUpdateVm addressVm)
        {
            var address = item.Addresses.FirstOrDefault(x => x.RefNbr == addressVm.RefNbr);
            if (address != null)
            {
                item.Addresses.Remove(address);
            }
            return address;
        }

        private CustomerAddress UpdateExistingAddress(Customer item, AddressUpdateVm addressVm)
        {
            var address = item.Addresses.FirstOrDefault(x => x.RefNbr == addressVm.RefNbr);
            if (address != null)
            {
                address.City = addressVm.City;
                address.Country = addressVm.Country;
                address.PostalCode = addressVm.PostalCode;
                address.StreetAddress = addressVm.StreetAddress;
                repository.UpdateAuditData(address);
            }
            return address;
        }

        private CustomerAddress AddNewAddress(Customer item, AddressUpdateVm addressVm)
        {
            var address = new CustomerAddress
            {
                Id = Guid.NewGuid().ToString(),
                City = addressVm.City,
                Country = addressVm.Country,
                StreetAddress = addressVm.StreetAddress,
                RefNbr = addressVm.RefNbr,
                PostalCode = addressVm.PostalCode
            };
            repository.AddAuditData(address);
            item.Addresses.Add(address);
            return address;
        }
    }
}