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

        public virtual async Task<TDto> GetFirst(TRequest request = null)
        {
            var record = await repository.GetFirst();
            return ToDto(record);
        }

        public virtual async Task<TDto> GetLast(TRequest request = null)
        {
            var record = await repository.GetLast();
            return ToDto(record);
        }

        public virtual async Task<TDto> GetNext(TRequest request = null)
        {
            var record = await repository.GetNext(request.RefNbr);
            return ToDto(record);
        }

        public virtual async Task<TDto> GetPrevious(TRequest request = null)
        {
            var record = await repository.GetPrevious(request.RefNbr);
            return ToDto(record);
        }

        public virtual async Task<TDto> GetById(TRequest request)
        {
            var record = await repository.GetById(request.RefNbr);
            return ToDto(record);
        }

        public abstract Task<TDto> Add(TViewModel model);

        public virtual async Task<TDto> Delete(TRequest model)
        {
            var item = await repository.GetById(model.RefNbr);
            return ToDto(repository.Delete(item).Entity);
        }

        public abstract Task<TDto> Update(TViewModel model);

        public virtual async Task<int> SaveChanges()
        {
            return await repository.SaveChanges();
        }


    }
}