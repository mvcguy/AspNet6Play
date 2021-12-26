using Microsoft.EntityFrameworkCore;

namespace PlayConnectServer.CustomUserStore
{
    public class AppDbContext : DbContext
    {

        public DbSet<ApplicationUser> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // var applicationUser = builder.Entity<ApplicationUser>().ToTable("User");
        }
    }

}