using PlayWebApp.Services.AppManagement.ViewModels;
using PlayWebApp.Services.CustomerManagement.ViewModels;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Identity.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
#nullable disable

namespace PlayWebApp.Services.ModelExtentions
{
    public static class Converters
    {

        public static StockItemDto ToDto(this StockItem model)
        {
            if (model == null) return null;

            return new StockItemDto
            {
                ItemDescription = model.Description,
                RefNbr = model.RefNbr,
                Prices = model.StockItemPrices?.Select(x => x.ToDto()).ToList()
            };
        }

        public static StockItemPriceDto ToDto(this StockItemPrice model)
        {
            if (model == null) return null;

            return new StockItemPriceDto
            {
                BreakQty = model.BreakQty,
                EffectiveFrom = model.EffectiveFrom,
                ExpiresAt = model.ExpiresAt,
                UnitCost = model.UnitCost,
                UnitOfMeasure = model.UnitOfMeasure,
                StockItemRefNbr = model.StockItemId,
                RefNbr = model.RefNbr
            };
        }

        public static StockItem ToModel(this StockItemUpdateVm model)
        {
            if (model == null) return null;
            return new StockItem
            {
                RefNbr = model.RefNbr,
                Description = model.ItemDescription,
                StockItemPrices = model.ItemPrices?.Select(x => x.ToModel()).ToList(),
            };
        }

        public static StockItemPrice ToModel(this StockItemPriceUpdateVm model)
        {
            if (model == null) return null;
            return new StockItemPrice
            {
                BreakQty = model.BreakQty,
                RefNbr = model.Code,
                EffectiveFrom = model.EffectiveFrom,
                ExpiresAt = model.ExpiresAt,
                UnitCost = model.UnitCost,
                UnitOfMeasure = model.UnitOfMeasure,

            };
        }

        public static AddressDto ToDto(this Address model)
        {
            if (model == null) return null;
            return new AddressDto
            {
                AddressCode = model.RefNbr,
                StreetAddress = model.StreetAddress,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country,
                Key = model.Id,
            };
        }

        public static CustomerAddress ToModel(this AddressUpdateVm model)
        {
            if (model == null) return null;
            return new CustomerAddress
            {
                City = model.City,
                RefNbr = model.AddressCode,
                Country = model.Country,
                PostalCode = model.PostalCode,
                StreetAddress = model.StreetAddress,

            };
        }

        public static BookingDto ToDto(this Booking model)
        {
            if (model == null) return null;
            return new BookingDto
            {
                RefNbr = model.RefNbr,
                Description = model.Description,
                Lines = model.BookingItems?.Select(x => x.ToDto()).ToList()
            };
        }

        public static BookingItemDto ToDto(this BookingItem model)
        {
            if (model == null) return null;
            return new BookingItemDto
            {
                RefNbr = model.RefNbr,
                BookingRefNbr = model.RefNbr, // TODO: need a fix!!!
                Description = model.Description,
                Discount = model.Discount ?? 0,
                ExtCost = model.ExtCost ?? 0,
                Quantity = model.Quantity ?? 0,
                StockItemRefNbr = model.StockItemId,
                UnitCost = model.UnitCost ?? 0,
            };
        }

        public static BookingItem ToModel(this BookingItemUpdateVm model)
        {
            if (model == null) return null;
            return new BookingItem
            {
                BookingId = model.BookingRefNbr,
                RefNbr = model.RefNbr,
                Description = model.Description,
                Discount = model.Discount,
                ExtCost = model.ExtCost,
                Quantity = model.Quantity,
                StockItemId = model.StockItemId,

                UnitCost = model.UnitCost,
                CreatedBy = model.UserId,

            };
        }

        public static ApplicationUserDto ToDto(this ApplicationUser model)
        {
            if (model == null) return null;
            return new ApplicationUserDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Key = model.CreatedBy,
                TenantId = model.TenantId
            };
        }

        public static TenantDto ToDto(this Tenant model)
        {
            if (model == null) return null;

            return new TenantDto
            {
                Key = model.Id,
                Code = model.TenantCode,
                Name = model.TenantName,
                Country = model.Country
            };
        }

        public static CustomerDto ToDto(this Customer model)
        {
            if (model == null) return null;

            return new CustomerDto
            {
                RefNbr = model.RefNbr,
                Active = model.Active,
                Name = model.Name,
            };
        }

        public static void AddAuditData<TModel>(this TModel model, string tenantId, string userId) where TModel : EntityBase
        {
            model.TenantId = tenantId;
            model.ModifiedOn = DateTime.UtcNow;
            model.CreatedBy = userId;
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