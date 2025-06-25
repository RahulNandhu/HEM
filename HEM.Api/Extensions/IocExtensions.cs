using HEM.Api.Extensions.Wrapper;
using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HEM.Api.Extensions;


public static partial class IocExtensions
{
    public static void AddApiServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddHttpContextAccessor();
        services.AddFrameworkServices();
        services.AddHttpClient();
        services.AddModuleServices();
        services.RegisterDbContext(builder.Configuration);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private static void AddFrameworkServices(this IServiceCollection services)
    {
        // Compress the response
        services.AddResponseCompression(options =>
        {
            options.Providers.Add<GzipCompressionProvider>();
            options.EnableForHttps = true;
            options.MimeTypes =
                        ["application/json", "text/tab-separated-values", "application/javascript", "text/csv", "text"];
        });

        services.AddControllers();

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        services.AddCors(options =>
        {
            options.AddPolicy(
                "CorsPolicy",
                builder =>
                {
                    builder
                        .WithOrigins("https://ui.buduportal.com", "http://localhost:3000", "https://demo.ui.buduportal.com", "https://staging.ui.buduportal.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });
    }

    private static void AddModuleServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
    }
}
