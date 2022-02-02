using PlayWebApp.Services.Logistics.LocationMgt.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels;

namespace PlayWebApp.Services.Logistics.CustomerManagement.ViewModels
{
    public class CustomerDto : BaseDto
    {
        public string Name { get; set; }

        public bool Active { get; set; }

        public DtoCollection<AddressDto> Addresses { get; set; }
    }


}