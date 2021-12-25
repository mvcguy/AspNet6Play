using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
using PlayWebApp.Services.Logistics.ViewModels.Requests;

namespace PlayWebApp.Services.CustomerManagement.ViewModels
{
    public class CustomerUpdateVm : ViewModelBase
    {
        public string Name { get; set; }

        public bool Active { get; set; }
    }

    public class CustomerDto : BaseDto
    {
        public string Name { get; set; }

        public bool Active { get; set; }
    }

    public class CustomerRequestDto : RequestBase
    {
        
    }


}