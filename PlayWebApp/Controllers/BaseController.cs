using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
#nullable disable

namespace PlayWebApp.Controllers
{
    [Authorize]
    [ApiController]
    public class BaseController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public BaseController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        protected virtual string UserId
        {
            get
            {
                try
                {
                    return User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                catch
                {
                    return null;
                }

            }
        }

        protected virtual string TenantId
        {
            get
            {
                try
                {
                    return User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                catch
                {
                    return null;
                }
            }
        }

        protected virtual IQueryable<TModel> GetTenantBasedQuery<TModel>() where TModel : EntityBase
        {
            return dbContext.Set<TModel>().Where(x => x.TenantId == TenantId);
        }

        protected virtual EntityEntry<TModel> Update<TModel>(TModel model) where TModel : EntityBase
        {
            UpdateAuditData(model);
            return dbContext.Set<TModel>().Update(model);
        }

        protected virtual EntityEntry<TModel> Add<TModel>(TModel model) where TModel : EntityBase
        {
            AddAuditData(model);
            return dbContext.Set<TModel>().Add(model);
        }

        protected virtual EntityEntry<TModel> Delete<TModel>(TModel model) where TModel : EntityBase
        {
            return dbContext.Set<TModel>().Remove(model);
        }

        protected virtual async Task<int> SaveChanges()
        {
            return await dbContext.SaveChangesAsync();
        }

        private void AddAuditData<TModel>(TModel model) where TModel : EntityBase
        {
            model.TenantId = TenantId;
            model.UserId = UserId;
            model.ModifiedOn = DateTime.UtcNow;
            model.UserId = UserId;
            model.ModifiedBy = UserId;
            model.CreatedOn = DateTime.UtcNow;
        }

        private void UpdateAuditData<TModel>(TModel model) where TModel : EntityBase
        {            
            model.UserId = UserId;
            model.ModifiedOn = DateTime.UtcNow;            
            model.ModifiedBy = UserId;            
        }

        protected virtual string DefaultAddressId
        {
            get
            {
                try
                {
                    var userExt = dbContext.Set<IdentityUserExt>().FirstOrDefault(x => x.UserId == UserId);
                    return userExt?.DefaultAddressId;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}