using System.Linq.Expressions;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
using PlayWebApp.Services.Logistics.ViewModels.Requests;
#nullable disable

namespace PlayWebApp.Services.DataNavigation
{
    public abstract class NavigationService<TDbModel, TRequest, TViewModel, TDto>
    where TDbModel : EntityBase, new()
    where TRequest : RequestBase
    where TViewModel : ViewModelBase
    where TDto : BaseDto

    {
        protected readonly INavigationRepository<TDbModel> repository;

        protected virtual int MaxPerPage { get; set; }

        private int DefaultMaxPerPage = 3;

        public NavigationService(INavigationRepository<TDbModel> repository)
        {
            this.repository = repository;
            this.MaxPerPage = this.MaxPerPage <= 0 ? DefaultMaxPerPage : this.MaxPerPage;
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

        // public virtual async Task<DtoCollection<TDto>> GetAllByParentId(string parentId, int page = 1)
        // {

        //     var pagedResult = await repository.GetAllByParentId(parentId, page, MaxPerPage);
        //     var result = new DtoCollection<TDto>()
        //     {
        //         Items = pagedResult.Records.Select(x => ToDto(x)),
        //         MetaData = new CollectionMetaData
        //         {
        //             PageSize = pagedResult.PageSize,
        //             PageIndex = pagedResult.PageIndex,
        //             TotalRecords = pagedResult.TotalRecords,
        //         }
        //     };
        //     return result;
        // }

        public virtual async Task<DtoCollection<TDto>> GetPaginatedCollection(Expression<Func<TDbModel, bool>> filter, int page = 1)
        {
                var pagedResult = await repository.GetPaginatedCollection(filter, page, MaxPerPage);
            var result = new DtoCollection<TDto>()
            {
                Items = pagedResult.Records.Select(x => ToDto(x)),
                MetaData = new CollectionMetaData
                {
                    PageSize = pagedResult.PageSize,
                    PageIndex = pagedResult.PageIndex,
                    TotalRecords = pagedResult.TotalRecords,
                }
            };
            return result;
        }

        public virtual async Task<DtoCollection<TDto>> GetAll(int page = 1)
        {
            var pagedResult = await repository.GetAll(page, MaxPerPage);
            var result = new DtoCollection<TDto>()
            {
                Items = pagedResult.Records.Select(x => ToDto(x)),
                MetaData = new CollectionMetaData
                {
                    PageSize = pagedResult.PageSize,
                    PageIndex = pagedResult.PageIndex,
                    TotalRecords = pagedResult.TotalRecords,
                }
            };
            return result;
        }

    }
}