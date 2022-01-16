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

        public virtual int MaxPerPage { get; set; }

        private int DefaultMaxPerPage = 3;

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

        public virtual async Task<DtoCollection<TDto>> GetAllByParentId(string parentId, int page = 1)
        {
            int take, skip;
            GetPagingInfo(page, out take, out skip);

            var items = await repository.GetAllByParentId(parentId, take, skip);
            var result = new DtoCollection<TDto>() { Items = items.Select(x => ToDto(x)) };
            return result;
        }

        public virtual async Task<DtoCollection<TDto>> GetAll(int page = 1)
        {
            int take, skip;
            GetPagingInfo(page, out take, out skip);

            var items = await repository.GetAll(take, skip);
            var result = new DtoCollection<TDto>() { Items = items.Select(x => ToDto(x)) };
            return result;
        }

        private void GetPagingInfo(int page, out int take, out int skip)
        {
            var iPage = page;
            if (page <= 0)
            {
                iPage = 1;
            }

            if (MaxPerPage <= 0)
            {
                MaxPerPage = DefaultMaxPerPage;
            }

            take = MaxPerPage;
            skip = 0;
            if (iPage > 1)
            {
                skip = (iPage - 1) * MaxPerPage;
            }
        }
    }
}