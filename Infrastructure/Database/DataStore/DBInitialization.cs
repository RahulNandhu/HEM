
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public static class DBInitialization
    {
        public static void UpdateToLatestVersion(this HemDbContext context)
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
    }
}
