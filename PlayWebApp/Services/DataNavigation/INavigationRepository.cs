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
        IQueryable<TModel> GetTenantBasedQuery(bool includeSubItems = true);
        Task<int> SaveChanges();
        EntityEntry<TModel> Update(TModel model);
    }
}