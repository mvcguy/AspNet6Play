using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
#nullable disable

namespace PlayWebApp.Services.DataNavigation
{
    public abstract class NavigationRepository<TModel> : INavigationRepository<TModel>
    where TModel : EntityBase, new()
    {
        protected readonly ApplicationDbContext dbContext;
        protected readonly IPlayAppContext context;

        public NavigationRepository(ApplicationDbContext dbContext, IPlayAppContext context)
        {
            this.dbContext = dbContext;
            this.context = context;
            ThrowOnEmptyContext();
        }

        public virtual void ThrowOnEmptyContext()
        {
            if (string.IsNullOrWhiteSpace(context.TenantId) || string.IsNullOrWhiteSpace(context.UserId))
                throw new Exception("Failed to access tenant or user info");
        }

        public abstract IQueryable<TModel> GetTenantBasedQuery(bool includeSubItems = false);

        public abstract IQueryable<TModel> GetQueryByParentId(string parentId);

        public virtual async Task<TModel> GetFirst()
        {
            return await GetTenantBasedQuery().OrderBy(x => x.RefNbr).FirstOrDefaultAsync();
        }

        public virtual async Task<TModel> GetLast()
        {
            return await GetTenantBasedQuery().OrderByDescending(x => x.RefNbr).FirstOrDefaultAsync();
        }

        public virtual async Task<TModel> GetNext(string refNbr)
        {
            TModel record;

            if (string.IsNullOrWhiteSpace(refNbr))
            {
                record = await GetTenantBasedQuery().OrderBy(x => x.RefNbr).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                record = await GetTenantBasedQuery().OrderBy(x => x.RefNbr)
                            .Where(x => x.RefNbr.CompareTo(refNbr) > 0).
                            Take(1).FirstOrDefaultAsync();
            }

            return record;
        }

        public virtual async Task<TModel> GetPrevious(string refNbr)
        {
            TModel record;

            if (string.IsNullOrWhiteSpace(refNbr))
            {
                record = await GetTenantBasedQuery().OrderByDescending(x => x.RefNbr).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                record = await GetTenantBasedQuery().OrderByDescending(x => x.RefNbr)
                            .Where(x => x.RefNbr.CompareTo(refNbr) < 0).
                            Take(1).FirstOrDefaultAsync();
            }

            return record;
        }

        public virtual async Task<TModel> GetById(string refNbr)
        {
            return await GetTenantBasedQuery().FirstOrDefaultAsync(x => x.RefNbr == refNbr);
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

        public virtual async Task<PagedResult<TModel>> GetAll(int pageIndex, int pageSize)
        {
            GetPagingInfo(pageIndex, pageSize, out var take, out var skip);
            var totalRecords = GetTenantBasedQuery().Count();
            var records = await GetTenantBasedQuery().OrderBy(x => x.RefNbr).Skip(skip).Take(take).ToListAsync();

            return new PagedResult<TModel>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Records = records
            };
        }

        public async Task<PagedResult<TModel>> GetAllByParentId(string parentId, int pageIndex, int pageSize)
        {
            GetPagingInfo(pageIndex, pageSize, out var take, out var skip);
            var totalRecords = GetQueryByParentId(parentId).Count();
            var records = await GetQueryByParentId(parentId).OrderBy(x => x.RefNbr).Skip(skip).Take(take).ToListAsync();
            return new PagedResult<TModel>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Records = records
            };
        }

        public async Task<IEnumerable<TModel>> GetAllByParentId(string parentId)
        {
            return await GetQueryByParentId(parentId).OrderBy(x => x.RefNbr).ToListAsync();
        }

        public virtual async Task<int> SaveChanges()
        {
            return await dbContext.SaveChangesAsync();
        }

        public void AddAuditData<TEntity>(TEntity model) where TEntity : EntityBase
        {
            model.ModifiedOn = DateTime.UtcNow;
            model.ModifiedBy = context.UserId;
            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = context.UserId;
            model.TenantId = context.TenantId;
        }

        public void UpdateAuditData<TEntity>(TEntity model) where TEntity : EntityBase
        {
            model.ModifiedOn = DateTime.UtcNow;
            model.ModifiedBy = context.UserId;
        }

        private void GetPagingInfo(int page, int pageSize, out int take, out int skip)
        {
            var iPage = page;
            if (page <= 0)
            {
                iPage = 1;
            }

            take = pageSize;
            skip = 0;
            if (iPage > 1)
            {
                skip = (iPage - 1) * pageSize;
            }
        }

    }

    public class PagedResult<TModel> where TModel : EntityBase, new()
    {
        public IEnumerable<TModel> Records { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }


    }
}