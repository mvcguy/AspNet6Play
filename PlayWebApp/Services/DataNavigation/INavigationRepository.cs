using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.Database.Model;
#nullable disable

namespace PlayWebApp.Services.DataNavigation
{
    public interface INavigationRepository<TModel> where TModel : EntityBase
    {
        EntityEntry<TModel> Add(TModel model);
        EntityEntry<TModel> Delete(TModel model);
        Task<TModel> GetById(string refNbr);
        Task<TModel> GetFirst();
        Task<TModel> GetLast();
        Task<TModel> GetNext(string refNbr);
        Task<TModel> GetPrevious(string refNbr);

        Task<IEnumerable<TModel>> GetAll(int take, int skip);

        Task<IEnumerable<TModel>> GetAllByParentId(string parentId);

        Task<IEnumerable<TModel>> GetAllByParentId(string parentId, int take, int skip);

        IQueryable<TModel> GetTenantBasedQuery(bool includeSubItems = true);
        Task<int> SaveChanges();
        EntityEntry<TModel> Update(TModel model);

        void AddAuditData<TEntity>(TEntity model) where TEntity : EntityBase;

        void UpdateAuditData<TEntity>(TEntity model) where TEntity : EntityBase;
    }
}