#nullable disable
using PlayWebApp;

namespace PlayWebApp.Services.Logistics.ViewModels.Dtos
{
    public class AddressDto : BaseDto
    {

        public string Key { get; set; }

        public string AddressCode { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public bool PreferredAddress { get; set; }
    }

}