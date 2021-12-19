using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.GenericModels;
using PlayWebApp.Services.Identity.Repository;
using PlayWebApp.Services.Identity.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
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

        public async Task<ApplicationUserDto> GetUser(string userId)
        {
            var record = await repository.GetUser(userId);
            return record.ToDto();
        }


        public async Task<OperationResult> CreateUser(ApplicationUserUpdateVm model)
        {
            try
            {
                var tenant = await appMgtService.GetTenantByCode(model.TenantCode);

                if (tenant == null)
                    return OperationResult.Failure(new Exception("Tenant not found"));

                var appUser = await repository.GetUser(model.UserId);
                if (appUser != null) return OperationResult.Success;

                var userId = Guid.NewGuid().ToString();

                var address = await repository.GetUserAddressByCode(model.UserId, model.DefaultAddress.AddressCode);
                if (address == null)
                {
                    address = new Address
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedBy = model.UserId,
                        Code = model.DefaultAddress.AddressCode,
                        StreetAddress = model.DefaultAddress.StreetAddress,
                        City = model.DefaultAddress.City,
                        PostalCode = model.DefaultAddress.PostalCode,
                        Country = model.DefaultAddress.Country,
                    };

                    address.AddAuditData(tenant.Key, userId);
                    var result = repository.CreateUserAddress(address);
                    if (!result.Succeeded)
                        return OperationResult.Failure(new Exception("Cannot create user default address"));
                }

                appUser = new ApplicationUser
                {
                    Id = userId,
                    CreatedBy = model.UserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DefaultAddressId = address?.Id,
                };

                appUser.AddAuditData(tenant.Key, userId);
                var usrResult = repository.CreateUser(appUser);
                if (!usrResult.Succeeded)
                    return OperationResult.Failure(new Exception("Cannot create application user"));

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

        public async Task<OperationResult> UpdateUser(ApplicationUserUpdateVm model)
        {
            try
            {
                var appUser = await repository.GetUser(model.UserId);
                if (appUser == null)
                    return OperationResult.Failure(new Exception("Record not found"));

                appUser.FirstName = model.FirstName;
                appUser.LastName = model.LastName;
                repository.UpdateUser(appUser);

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
                    appUser.DefaultAddressId = address.Id;

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