using PlayWebApp.Services.CustomerManagement.ViewModels;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.ModelExtentions;

namespace PlayWebApp.Services.CustomerManagement
{
    public class CustomerService : NavigationService<Customer, CustomerRequestDto, CustomerUpdateVm, CustomerDto>
    {
        public CustomerService(INavigationRepository<Customer> repository) : base(repository)
        {
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
            };
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

            var entry = repository.Update(item);

            return entry.Entity.ToDto();

        }
    }
}