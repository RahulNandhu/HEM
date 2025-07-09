namespace HEM.Api.Extensions.Wrapper;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


public static class DatabaseWrapper
{
    public static void RegisterDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<HemDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    }

    //public static void RegisterDbContext(this IServiceCollection services, IConfiguration configuration)
    //{
    //    var connectionString = configuration.GetConnectionString("DefaultConnection");

    //    services.AddDbContext<HemDbContext>(options =>
    //        options.UseSqlServer(connectionString));
    //}

    public static void InitializeDatabase(this IServiceScope serviceScope)
    {
        var context = serviceScope.ServiceProvider.GetService<HemDbContext>();
        context.UpdateToLatestVersion();
    }
}
