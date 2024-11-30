using Microsoft.EntityFrameworkCore;
using dotnet_web_api.Model;
namespace dotnet_web_api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User>? Users { get; set; }
        
        public DbSet<AuthLog>? AuthLogs { get; set; }
    }
}