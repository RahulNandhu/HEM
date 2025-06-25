namespace Infrastructure;

using Microsoft.EntityFrameworkCore;
using System.Reflection;

public class HemDbContext : DbContext
{
    public HemDbContext(DbContextOptions<HemDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
