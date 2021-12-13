using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.Database.Model;
#nullable disable

namespace PlayWebApp.Services.DataNavigation
{
    public interface INavigationRepository<TModel> where TModel : EntityBase
    {
        EntityEntry<TModel> Add(TModel model, string userId);
        EntityEntry<TModel> Delete(TModel model);
        Task<TModel> GetById(string tenantId, string refNbr);
        Task<TModel> GetFirst(string tenantId);
        Task<TModel> GetLast(string tenantId);
        Task<TModel> GetNext(string tenantId, string refNbr);
        Task<TModel> GetPrevious(string tenantId, string refNbr);
        IQueryable<TModel> GetTenantBasedQuery(string tenantId, bool includeSubItems = true);
        Task<int> SaveChanges();
        EntityEntry<TModel> Update(TModel model, string userId);
    }
}