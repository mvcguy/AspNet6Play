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
                if (appUser != null) return OperationResult.Success(appUser.Id);

                var userId = Guid.NewGuid().ToString();

                appUser = new ApplicationUser
                {
                    Id = userId,
                    CreatedBy = model.UserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    Email = model.Email,
                };

                appUser.AddAuditData(tenant.Key, userId);
                var usrResult = repository.CreateUser(appUser);
                if (!usrResult.Succeeded)
                    return OperationResult.Failure(new Exception("Cannot create application user"));

                return usrResult;
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
                return repository.UpdateUser(appUser);

            }
            catch (Exception e)
            {
                return OperationResult.Failure(e);
            }

        }

        public async Task<OperationResult> SaveChanges()
        {
            return await repository.SaveChanges();
        }

    }
}