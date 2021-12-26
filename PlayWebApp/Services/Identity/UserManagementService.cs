using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.GenericModels;
using PlayWebApp.Services.Identity.Repository;
using PlayWebApp.Services.Identity.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
using PlayWebApp.Services.ModelExtentions;
#nullable disable

namespace PlayWebApp.Services.Identity
{
    public class UserService : NavigationService<ApplicationUser, AppUserRequestDto, AppUserUpdateVm, AppUserDto>
    {
        public UserService(INavigationRepository<ApplicationUser> repository) : base(repository)
        {
        }

        public override async Task<AppUserDto> Add(AppUserUpdateVm model)
        {
            var item = await repository.GetById(model.RefNbr);
            if (item != null) throw new Exception("Record exist from before");

            item = new ApplicationUser
            {
                RefNbr = model.RefNbr,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName
            };

            var record = repository.Add(item);
            return record.Entity.ToDto();
        }

        public override AppUserDto ToDto(ApplicationUser model)
        {
            return model.ToDto();
        }

        public override async Task<AppUserDto> Update(AppUserUpdateVm model)
        {
            var item = await repository.GetById(model.RefNbr);
            if (item == null) throw new Exception("Record does not exist");

            item.FirstName = model.FirstName;
            item.LastName = model.LastName;
            item.UserName = model.UserName;
            item.Email = model.Email;

            var record = repository.Update(item);
            return record.Entity.ToDto();
        }
    }
}