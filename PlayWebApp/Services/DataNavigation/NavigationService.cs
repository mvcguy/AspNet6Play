using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
using PlayWebApp.Services.Logistics.ViewModels.Requests;
#nullable disable

namespace PlayWebApp.Services.DataNavigation
{
    public abstract class NavigationService<TDbModel, TRequest, TViewModel, TDto>
    where TDbModel : EntityBase
    where TRequest : RequestBase
    where TViewModel : ViewModelBase
    where TDto : BaseDto

    {
        protected readonly INavigationRepository<TDbModel> repository;

        public NavigationService(INavigationRepository<TDbModel> repository)
        {
            this.repository = repository;
        }

        public abstract TDto ToDto(TDbModel model);

        public abstract TDbModel ToDbModel(TViewModel vm);

        public virtual async Task<TDto> GetFirst(TRequest request = null)
        {
            var record = await repository.GetFirst(request.TenantId);
            return ToDto(record);
        }

        public virtual async Task<TDto> GetLast(TRequest request = null)
        {
            var record = await repository.GetLast(request.TenantId);
            return ToDto(record);
        }

        public virtual async Task<TDto> GetNext(TRequest request = null)
        {
            var record = await repository.GetNext(request.TenantId, request.RefNbr);
            return ToDto(record);
        }

        public virtual async Task<TDto> GetPrevious(TRequest request = null)
        {
            var record = await repository.GetPrevious(request.TenantId, request.RefNbr);
            return ToDto(record);
        }

        public virtual async Task<TDto> GetById(TRequest request)
        {
            var record = await repository.GetById(request.TenantId, request.RefNbr);
            return ToDto(record);
        }

        public virtual TDto Add(TViewModel model, string userId)
        {
            var entity = repository.Add(ToDbModel(model), userId).Entity;
            return ToDto(entity);
        }

        public virtual async Task<TDto> Delete(TRequest model)
        {
            var item = await repository.GetById(model.TenantId, model.RefNbr);
            return ToDto(repository.Delete(item).Entity);
        }

        public virtual TDto Update(TViewModel model, string userId)
        {
            var entity = repository.Update(ToDbModel(model), userId).Entity;
            return ToDto(entity);
        }

        public virtual async Task<int> SaveChanges()
        {
            return await repository.SaveChanges();
        }


    }
}