using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.GenericModels;
#nullable disable

namespace PlayWebApp.Services.AppManagement.Repository
{
    public class TenantRepository : NavigationRepository<Tenant>
    {
        public TenantRepository(ApplicationDbContext dbContext, IPlayAppContext context) : base(dbContext, context)
        {
        }

        public override IQueryable<Tenant> GetQueryByParentId(string parentId)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<Tenant> GetTenantBasedQuery(bool includeSubItems = true)
        {
            return dbContext.Tenants;
        }

        public override void ThrowOnEmptyContext()
        {
            /* ignore for this repository */
        }
    }
}