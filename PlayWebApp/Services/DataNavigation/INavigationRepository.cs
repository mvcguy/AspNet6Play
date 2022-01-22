using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.Database.Model;
#nullable disable

namespace PlayWebApp.Services.DataNavigation
{
    public interface INavigationRepository<TModel> where TModel : EntityBase, new()
    {
        EntityEntry<TModel> Add(TModel model);
        EntityEntry<TModel> Delete(TModel model);
        Task<TModel> GetById(string refNbr);
        Task<TModel> GetFirst();
        Task<TModel> GetLast();
        Task<TModel> GetNext(string refNbr);
        Task<TModel> GetPrevious(string refNbr);

        Task<PagedResult<TModel>> GetAll(int pageIndex, int pageSize);

        IQueryable<TModel> GetQuery();
        Task<int> SaveChanges();
        EntityEntry<TModel> Update(TModel model);

        void AddAuditData<TEntity>(TEntity model) where TEntity : EntityBase;

        void UpdateAuditData<TEntity>(TEntity model) where TEntity : EntityBase;
        Task<PagedResult<TModel>> GetPaginatedCollection(Expression<Func<TModel, bool>> filter, int page, int pageLength);
        Task<IEnumerable<TModel>> GetCollection(Expression<Func<TModel, bool>> filter);
    }
}