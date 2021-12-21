using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
#nullable disable

namespace PlayWebApp.Services.DataNavigation
{
    public abstract class NavigationRepository<TModel> : INavigationRepository<TModel> where TModel : EntityBase
    {
        protected readonly ApplicationDbContext dbContext;
        protected readonly IPlayAppContext context;

        public NavigationRepository(ApplicationDbContext dbContext, IPlayAppContext context)
        {
            this.dbContext = dbContext;
            this.context = context;

            if (string.IsNullOrWhiteSpace(context.TenantId) || string.IsNullOrWhiteSpace(context.UserId))
                throw new Exception("Failed to access tenant or user info");
        }

        public abstract IQueryable<TModel> GetTenantBasedQuery(bool includeSubItems = true);

        public virtual async Task<TModel> GetFirst()
        {
            return await GetTenantBasedQuery().OrderBy(x => x.Code).FirstOrDefaultAsync();
        }

        public virtual async Task<TModel> GetLast()
        {
            return await GetTenantBasedQuery().OrderByDescending(x => x.Code).FirstOrDefaultAsync();
        }

        public virtual async Task<TModel> GetNext(string refNbr)
        {
            TModel record;

            if (string.IsNullOrWhiteSpace(refNbr))
            {
                record = await GetTenantBasedQuery().OrderBy(x => x.Code).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                record = await GetTenantBasedQuery().OrderBy(x => x.Code)
                            .Where(x => x.Code.CompareTo(refNbr) > 0).
                            Take(1).FirstOrDefaultAsync();
            }

            return record;
        }

        public virtual async Task<TModel> GetPrevious(string refNbr)
        {
            TModel record;

            if (string.IsNullOrWhiteSpace(refNbr))
            {
                record = await GetTenantBasedQuery().OrderByDescending(x => x.Code).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                record = await GetTenantBasedQuery().OrderByDescending(x => x.Code)
                            .Where(x => x.Code.CompareTo(refNbr) < 0).
                            Take(1).FirstOrDefaultAsync();
            }

            return record;
        }

        public virtual async Task<TModel> GetById(string refNbr)
        {
            return await GetTenantBasedQuery().FirstOrDefaultAsync(x => x.Code == refNbr);
        }

        public virtual EntityEntry<TModel> Update(TModel model)
        {
            UpdateAuditData(model);
            return dbContext.Set<TModel>().Update(model);
        }

        public virtual EntityEntry<TModel> Add(TModel model)
        {
            AddAuditData(model);
            model.Id = Guid.NewGuid().ToString();
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

        protected virtual void AddAuditData(TModel model)
        {
            model.ModifiedOn = DateTime.UtcNow;
            model.ModifiedBy = context.UserId;
            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = context.UserId;
            model.TenantId = context.TenantId;
        }

        protected virtual void UpdateAuditData(TModel model)
        {
            model.ModifiedOn = DateTime.UtcNow;
            model.ModifiedBy = context.UserId;
        }


    }
}