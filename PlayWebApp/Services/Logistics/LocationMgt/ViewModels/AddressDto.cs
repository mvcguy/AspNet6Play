#nullable disable
using PlayWebApp;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;

namespace PlayWebApp.Services.Logistics.LocationMgt.ViewModels
{
    public class AddressDto : BaseDto
    {
        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public bool IsDefault { get; set; }        
    }

}