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
                AddressCode = model.Code,
                StreetAddress = model.StreetAddress,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country
            };
        }

        public static BookingDto ToBookingDto(this Booking model)
        {
            return new BookingDto { BookingNumber = model.Code, Description = model.Description };
        }
    }

}