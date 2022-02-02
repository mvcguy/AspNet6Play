using PlayWebApp.Services.AppManagement.ViewModels;
using PlayWebApp.Services.Logistics.CustomerManagement.ViewModels;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Identity.ViewModels;
using PlayWebApp.Services.Logistics.BookingMgt.ViewModels;
using PlayWebApp.Services.Logistics.LocationMgt.ViewModels;
using PlayWebApp.Services.Logistics.InventoryMgt.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels;
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
                InternalId = model.Id,
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
                RefNbr = model.RefNbr,
                InternalId = model.Id,
            };
        }

         public static StockItemUpdateVm ToVm(this StockItemDto model)
        {
            if (model == null) return null;

            return new StockItemUpdateVm
            {
                ItemDescription = model.ItemDescription,
                RefNbr = model.RefNbr,
                ItemPrices = model.Prices?.Select(x => x.ToVm()).ToList()
            };
        }

        public static StockItemPriceUpdateVm ToVm(this StockItemPriceDto model)
        {
            if (model == null) return null;

            return new StockItemPriceUpdateVm
            {
                BreakQty = model.BreakQty,
                EffectiveFrom = model.EffectiveFrom,
                ExpiresAt = model.ExpiresAt,
                UnitCost = model.UnitCost,
                UnitOfMeasure = model.UnitOfMeasure,
                RefNbr = model.RefNbr,
            };
        }

        public static AddressDto ToDto(this Address model)
        {
            if (model == null) return null;
            return new AddressDto
            {
                RefNbr = model.RefNbr,
                StreetAddress = model.StreetAddress,
                City = model.City,
                PostalCode = model.PostalCode,
                InternalId = model.Id,
                Country = model.Country,
            };
        }

        public static AddressUpdateVm ToVm(this AddressDto model)
        {
            if (model == null) return null;
            return new AddressUpdateVm
            {
                RefNbr = model.RefNbr,
                StreetAddress = model.StreetAddress,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country,
                IsDefault = model.IsDefault
            };
        }

        public static BookingDto ToDto(this Booking model)
        {
            if (model == null) return null;
            return new BookingDto
            {
                RefNbr = model.RefNbr,
                Description = model.Description,
                InternalId = model.Id,
                CustomerRefNbr = model.Customer.RefNbr,
                Balance = model.Balance,
                Discount = model.Discount,
                LinesTotal = model.LinesTotal,
                TaxableAmount = model.TaxableAmount,
                TaxAmount = model.TaxAmount,
                TotalAmount = model.TotalAmount,
                Lines = model.BookingItems?.Select(x => x.ToDto()).ToList()
            };
        }

        public static BookingUpdateVm ToVm(this BookingDto model)
        {
            if (model == null) return null;
            return new BookingUpdateVm
            {
                RefNbr = model.RefNbr,
                Description = model.Description,
                CustomerRefNbr = model.CustomerRefNbr,
                Balance = model.Balance,
                Discount = model.Discount,
                LinesTotal = model.LinesTotal,
                TaxableAmount = model.TaxableAmount,
                TaxAmount = model.TaxAmount,
                TotalAmount = model.TotalAmount,
                Lines = model.Lines?.Select(x=>x.ToVm()).ToList()
            };
        }

        public static BookingItemDto ToDto(this BookingItem model)
        {
            if (model == null) return null;
            return new BookingItemDto
            {
                RefNbr = model.RefNbr,
                InternalId = model.Id,
                //BookingRefNbr = model.Booking.RefNbr, // TODO: need a fix!!!
                Description = model.Description,
                Discount = model.Discount ?? 0,
                ExtCost = model.ExtCost ?? 0,
                Quantity = model.Quantity ?? 0,
                StockItemRefNbr = model.StockItem?.RefNbr,
                UnitCost = model.UnitCost ?? 0,
            };
        }

        public static BookingItemUpdateVm ToVm(this BookingItemDto model)
        {
            return new BookingItemUpdateVm
            {
                Description = model.Description,
                Discount = model.Discount,
                ExtCost = model.ExtCost,
                Quantity = model.Quantity,
                RefNbr = model.RefNbr,
                StockItemRefNbr = model.StockItemRefNbr,
                UnitCost = model.UnitCost
            };
        }

        public static AppUserDto ToDto(this ApplicationUser model)
        {
            if (model == null) return null;
            return new AppUserDto
            {
                RefNbr = model.RefNbr,
                InternalId = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
        }

        public static TenantDto ToDto(this Tenant model)
        {
            if (model == null) return null;

            return new TenantDto
            {
                InternalId = model.Id,
                RefNbr = model.RefNbr,
                Name = model.TenantName,
                Country = model.Country
            };
        }

        public static CustomerDto ToDto(this Customer model)
        {
            if (model == null) return null;

            var result = new CustomerDto
            {
                RefNbr = model.RefNbr,
                InternalId = model.Id,
                Active = model.Active,
                Name = model.Name,
            };

            if (model.Addresses != null)
            {
                var defaultAddress = model.DefaultAddress?.RefNbr;
                result.Addresses = new DtoCollection<AddressDto>()
                {
                    Items = model.Addresses.Select(x =>
                    {
                        var dto = x.ToDto();
                        if (dto.RefNbr == defaultAddress) dto.IsDefault = true;
                        return dto;
                    }).ToList()
                };

            }

            return result;
        }

        public static CustomerUpdateVm ToVm(this CustomerDto model)
        {
            if (model == null) return null;

            var result = new CustomerUpdateVm
            {
                RefNbr = model.RefNbr,
                Active = model.Active,
                Name = model.Name,
            };

            if (model.Addresses != null && model.Addresses.Items != null)
            {
                result.Addresses = model.Addresses.Items.Select(x => x.ToVm()).ToList();
            }

            return result;

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