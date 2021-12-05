using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Logistics.ViewModels;

namespace PlayWebApp.Services.ModelExtentions
{
    public static class Converters
    {
        public static AddressDto ToAddressDto(this Address model)
        {
            return new AddressDto
            {
                AddressCode = model.AddressCode,
                StreetAddress = model.StreetAddress,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country
            };
        }
    }

}