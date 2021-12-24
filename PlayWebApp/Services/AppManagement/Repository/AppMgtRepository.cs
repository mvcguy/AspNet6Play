using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.GenericModels;
#nullable disable

namespace PlayWebApp.Services.AppManagement.Repository
{
    public class AppMgtRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly DbSet<Tenant> Tenants;

        public AppMgtRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            Tenants = dbContext.Set<Tenant>();
        }

        public async Task<IEnumerable<Tenant>> GetTenants(int top = 100, int skip = 0, string customerId = null)
        {
            // TODO: introduce customer dimension.

            return await Tenants.OrderBy(x => x.TenantCode).Skip(skip).Take(top).ToListAsync();
        }

        public async Task<Tenant> GetTenantByCode(string code)
        {
            return await Tenants.FirstOrDefaultAsync(x => x.TenantCode == code);
        }

        public OperationResult CreateTenant(Tenant tenant)
        {
            try
            {
                Tenants.Add(tenant);
                return OperationResult.Success(tenant.Id);
            }
            catch (Exception e)
            {
                return OperationResult.Failure(e);
            }
        }
    }
}