using HEM.Api.Interfaces;
using HEM.Api.Services;

namespace HEM.Api.Extensions
{
    public static partial class IocExtensions
    {
        private static void AddModuleServices(this IServiceCollection services)
        {
            services.AddScoped<INotificationService, NotificationServices>();
            services.AddScoped<IAiTaskService, AiTaskServices>();
            services.AddMemoryCache();

        }
    }
}
