using PlayWebApp.Services.CustomerManagement.ViewModels;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;

namespace PlayWebApp.Services.CustomerManagement
{
    public class CustomerService : NavigationService<Customer, CustomerRequestDto, CustomerUpdateVm, CustomerDto>
    {
        public CustomerService(INavigationRepository<Customer> repository) : base(repository)
        {
        }

        public override Task<CustomerDto> Add(CustomerUpdateVm model)
        {
            throw new NotImplementedException();
        }

        public override CustomerDto ToDto(Customer model)
        {
            throw new NotImplementedException();
        }

        public override Task<CustomerDto> Update(CustomerUpdateVm model)
        {
            throw new NotImplementedException();
        }
    }
}