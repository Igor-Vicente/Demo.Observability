using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Demo.Aspire.Data
{
    public class InsightsDbContext : IdentityDbContext
    {
        public InsightsDbContext(DbContextOptions<InsightsDbContext> options) : base(options)
        {
        }
    }
}
