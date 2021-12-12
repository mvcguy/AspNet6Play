using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.GenericModels;
using PlayWebApp.Services.Identity.Repository;
using PlayWebApp.Services.Identity.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.ModelExtentions;
#nullable disable

namespace PlayWebApp.Services.Identity
{
    public class UserManagementService
    {
        private readonly UserManagementRepository repository;
        private readonly AppMgtService appMgtService;

        public UserManagementService(UserManagementRepository repository, AppMgtService appMgtService)
        {
            this.repository = repository;
            this.appMgtService = appMgtService;
        }

        public async Task<IdentityUserExtDto> GetIdentityUserExt(string userId)
        {
            var record = await repository.GetUserExt(userId);
            return record.ToDto();
        }


        public async Task<OperationResult> CreateIdentityUserExt(IdentityUserUpdateVm model)
        {
            try
            {
                var tenant = await appMgtService.GetTenantByCode(model.TenantCode);

                if (tenant == null)
                    return OperationResult.Failure(new Exception("Tenant not found"));

                var userExt = await repository.GetUserExt(model.UserId);
                if (userExt != null) return OperationResult.Success;

                var address = await repository.GetUserAddressByCode(model.UserId, model.DefaultAddress.AddressCode);
                if (address == null)
                {
                    address = new Address
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = model.UserId,
                        Code = model.DefaultAddress.AddressCode,
                        StreetAddress = model.DefaultAddress.StreetAddress,
                        City = model.DefaultAddress.City,
                        PostalCode = model.DefaultAddress.PostalCode,
                        Country = model.DefaultAddress.Country,
                    };

                    address.AddAuditData(tenant.Key, model.UserId);
                    var result = repository.CreateUserAddress(address);
                    if (!result.Succeeded)
                        return OperationResult.Failure(new Exception("Cannot create user default address"));
                }

                userExt = new IdentityUserExt
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = model.UserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DefaultAddressId = address?.Id,
                };

                userExt.AddAuditData(tenant.Key, model.UserId);
                var usrResult = repository.CreateUserExt(userExt);
                if (!usrResult.Succeeded)
                    return OperationResult.Failure(new Exception("Cannot create user ext"));

                //
                // save changes
                // Changes are ATOMIC
                // ALL or NONE succeed!!!
                //
                return await repository.SaveChanges();
            }
            catch (Exception e)
            {
                return OperationResult.Failure(e);
            }
        }

        public async Task<OperationResult> UpdateIdentityUserExt(IdentityUserUpdateVm model)
        {
            try
            {
                var userExt = await repository.GetUserExt(model.UserId);
                if (userExt == null)
                    return OperationResult.Failure(new Exception("Record not found"));

                userExt.FirstName = model.FirstName;
                userExt.LastName = model.LastName;
                repository.UpdateUserExt(userExt);

                if (!string.IsNullOrWhiteSpace(model.DefaultAddress?.AddressCode))
                {
                    var address = await repository.GetUserAddressByCode(model.UserId, model.DefaultAddress.AddressCode);
                    if (address == null)
                    {
                        return OperationResult.Failure(new Exception("User default address does not exist"));
                    }
                    address.StreetAddress = model.DefaultAddress.StreetAddress;
                    address.City = model.DefaultAddress.City;
                    address.PostalCode = model.DefaultAddress.PostalCode;
                    address.Country = model.DefaultAddress.Country;

                    repository.UpdateUserAddress(address);
                    userExt.DefaultAddressId = address.Id;

                }

                //
                // save changes
                // Changes are ATOMIC
                // ALL or NONE succeed!!!
                //
                return await repository.SaveChanges();
            }
            catch (Exception e)
            {
                return OperationResult.Failure(e);
            }

        }

        public async Task<IEnumerable<AddressDto>> GetAllUserAddresses(string userId)
        {
            return (await repository.GetAllUserAddresses(userId)).Select(x => x.ToDto());
        }

        public async Task<AddressDto> GetUserDefaultAddress(string userId)
        {
            var record = await repository.GetUserDefaultAddress(userId);
            return record.ToDto();
        }

        public async Task<AddressDto> GetUserAddressById(string userId, string addressId)
        {
            var record = await repository.GetUserAddressById(userId, addressId);
            return record.ToDto();
        }

        public async Task<AddressDto> GetUserAddressByCode(string userId, string addressCode)
        {
            var record = await repository.GetUserAddressByCode(userId, addressCode);
            return record.ToDto();
        }

    }
}