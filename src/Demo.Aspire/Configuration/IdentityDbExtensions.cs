using Demo.Aspire.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Demo.Aspire.Configuration
{
    public static class IdentityDbExtensions
    {
        public static void AddIdentityAndDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<InsightsDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Sqlserver")));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<InsightsDbContext>();
        }
    }
}
