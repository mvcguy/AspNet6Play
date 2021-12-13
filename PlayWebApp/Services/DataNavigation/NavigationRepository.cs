using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
#nullable disable

namespace PlayWebApp.Services.DataNavigation
{
    public abstract class NavigationRepository<TModel> : INavigationRepository<TModel> where TModel : EntityBase
    {
         protected readonly ApplicationDbContext dbContext;

        public NavigationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public abstract IQueryable<TModel> GetTenantBasedQuery(string tenantId, bool includeSubItems = true);

        public virtual async Task<TModel> GetFirst(string tenantId)
        {
            return await GetTenantBasedQuery(tenantId).OrderBy(x => x.Code).FirstOrDefaultAsync(x => x.TenantId == tenantId);
        }

        public virtual async Task<TModel> GetLast(string tenantId)
        {
            return await GetTenantBasedQuery(tenantId).OrderByDescending(x => x.Code).FirstOrDefaultAsync(x => x.TenantId == tenantId);
        }

        public virtual async Task<TModel> GetNext(string tenantId, string refNbr)
        {
            TModel record;

            if (string.IsNullOrWhiteSpace(refNbr))
            {
                record = await GetTenantBasedQuery(tenantId).OrderBy(x => x.Code).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                record = await GetTenantBasedQuery(tenantId).OrderBy(x => x.Code)
                            .Where(x => x.Code.CompareTo(refNbr) > 0).
                            Take(1).FirstOrDefaultAsync();
            }

            return record;
        }

        public virtual async Task<TModel> GetPrevious(string tenantId, string refNbr)
        {
            TModel record;

            if (string.IsNullOrWhiteSpace(refNbr))
            {
                record = await GetTenantBasedQuery(tenantId).OrderByDescending(x => x.Code).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                record = await GetTenantBasedQuery(tenantId).OrderByDescending(x => x.Code)
                            .Where(x => x.Code.CompareTo(refNbr) < 0).
                            Take(1).FirstOrDefaultAsync();
            }

            return record;
        }

        public virtual async Task<TModel> GetById(string tenantId, string refNbr)
        {
            return await GetTenantBasedQuery(tenantId).FirstOrDefaultAsync(x => x.Code == refNbr);
        }

        public virtual EntityEntry<TModel> Update(TModel model, string userId)
        {
            UpdateAuditData(model, userId);
            return dbContext.Set<TModel>().Update(model);
        }

        public virtual EntityEntry<TModel> Add(TModel model, string userId)
        {
            AddAuditData(model, userId);
            return dbContext.Set<TModel>().Add(model);
        }

        public virtual EntityEntry<TModel> Delete(TModel model)
        {
            return dbContext.Set<TModel>().Remove(model);
        }

        public virtual async Task<int> SaveChanges()
        {
            return await dbContext.SaveChangesAsync();
        }

        protected virtual void AddAuditData(TModel model, string userId)
        {
            model.ModifiedOn = DateTime.UtcNow;
            model.UserId = userId;
            model.ModifiedBy = userId;
            model.CreatedOn = DateTime.UtcNow;
        }

        protected virtual void UpdateAuditData(TModel model, string userId)
        {

            model.ModifiedOn = DateTime.UtcNow;
            model.ModifiedBy = userId;
        }


    }
}