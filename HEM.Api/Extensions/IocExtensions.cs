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
        services.AddSignalR();
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
                        .WithOrigins("http://localhost:5174")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });
    }

}
