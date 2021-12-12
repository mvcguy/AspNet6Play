using PlayWebApp.Services.AppManagement.ViewModels;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Identity.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels;
#nullable disable

namespace PlayWebApp.Services.ModelExtentions
{
    public static class Converters
    {
        public static AddressDto ToDto(this Address model)
        {
            if (model == null) return null;
            return new AddressDto
            {
                AddressCode = model.Code,
                StreetAddress = model.StreetAddress,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country,
                Key = model.Id,
            };
        }

        public static BookingDto ToDto(this Booking model)
        {
            if (model == null) return null;
            return new BookingDto { BookingNumber = model.Code, Description = model.Description };
        }

        public static IdentityUserExtDto ToDto(this IdentityUserExt model)
        {
            if (model == null) return null;
            return new IdentityUserExtDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                DefaultAddressId = model.DefaultAddressId,
                Key = model.UserId,
                TenantId = model.TenantId
            };
        }

        public static TenantDto ToDto(this Tenant model)
        {
            if(model == null) return null;

            return new TenantDto
            {
                Key = model.Id,
                Code = model.TenantCode,
                Name = model.TenantName,
                Country = model.Country
            };
        }

        public static void AddAuditData<TModel>(this TModel model, string tenantId, string userId) where TModel : EntityBase
        {
            model.TenantId = tenantId;
            model.ModifiedOn = DateTime.UtcNow;
            model.UserId = userId;
            model.ModifiedBy = userId;
            model.CreatedOn = DateTime.UtcNow;
        }

        public static void UpdateAuditData<TModel>(this TModel model, string userId) where TModel : EntityBase
        {            
            model.ModifiedOn = DateTime.UtcNow;            
            model.ModifiedBy = userId;            
        }
    }

}