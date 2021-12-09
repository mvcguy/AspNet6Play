using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
#nullable disable

namespace PlayWebApp.Controllers
{
    public interface INavigationBaseController
    {
        Task<TModel> GetLastRecord<TModel>() where TModel : EntityBase;
        Task<TModel> GetNextRecord<TModel>(string currentRecord) where TModel : EntityBase;
        Task<TModel> GetPreviousRecord<TModel>(string currentRecord) where TModel : EntityBase;
        Task<TModel> GetRecord<TModel>(string code) where TModel : EntityBase;
        Task<TModel> GetTopRecord<TModel>() where TModel : EntityBase;
    }

    public class NavigationBaseController : BaseController, INavigationBaseController
    {
        private readonly ApplicationDbContext dbContext;

        public NavigationBaseController(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<TModel> GetRecord<TModel>(string code) where TModel : EntityBase
        {
            return await GetTenantBasedQuery<TModel>().FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<TModel> GetNextRecord<TModel>(string currentRecord) where TModel : EntityBase
        {
            TModel record;

            if (string.IsNullOrWhiteSpace(currentRecord))
            {
                record = await GetTenantBasedQuery<TModel>().OrderBy(x => x.Code).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                record = await GetTenantBasedQuery<TModel>().OrderBy(x => x.Code)
                            .Where(x => x.Code.CompareTo(currentRecord) > 0).
                            Take(1).FirstOrDefaultAsync();
            }

            return record;

        }

        public async Task<TModel> GetPreviousRecord<TModel>(string currentRecord) where TModel : EntityBase
        {
            TModel record;

            if (string.IsNullOrWhiteSpace(currentRecord))
            {
                record = await GetTenantBasedQuery<TModel>().OrderByDescending(x => x.Code).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                record = await GetTenantBasedQuery<TModel>().OrderByDescending(x => x.Code)
                            .Where(x => x.Code.CompareTo(currentRecord) < 0).
                            Take(1).FirstOrDefaultAsync();
            }

            return record;
        }

        public async Task<TModel> GetTopRecord<TModel>() where TModel : EntityBase
        {
            return await GetTenantBasedQuery<TModel>().OrderBy(x => x.Code).FirstOrDefaultAsync();
        }

        public async Task<TModel> GetLastRecord<TModel>() where TModel : EntityBase
        {
            return await GetTenantBasedQuery<TModel>().OrderByDescending(x => x.Code).FirstOrDefaultAsync();
        }
    }
}