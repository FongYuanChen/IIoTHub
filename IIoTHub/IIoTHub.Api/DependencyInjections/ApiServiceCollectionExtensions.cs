using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace IIoTHub.Api.DependencyInjections
{
    /// <summary>
    /// 擴充方法，用於註冊 IIoTHub API 與 Swagger 設定
    /// </summary>
    public static class ApiServiceCollectionExtensions
    {
        /// <summary>
        /// 註冊 IIoTHub API 與 Swagger
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddIIoTHubApi(this IServiceCollection services)
        {
            services.AddControllers()
                    .AddApplicationPart(typeof(ApiServiceCollectionExtensions).Assembly)
                    .AddJsonOptions(options =>
                    {
                        // 保持原始大小寫，不使用 camelCase
                        options.JsonSerializerOptions.PropertyNamingPolicy = null;
                        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
                    });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("iiot-hub-v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "IIoTHub API"
                });

                var xmlFilePaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml")
                                            .Where(path => Path.GetFileName(path).StartsWith("IIoTHub"))
                                            .ToList();
                foreach (var xmlFilePath in xmlFilePaths)
                {
                    options.IncludeXmlComments(xmlFilePath, includeControllerXmlComments: true);
                }
            });

            return services;
        }

        /// <summary>
        /// 註冊 IIoTHub API 的 Swagger UI Endpoint
        /// </summary>
        /// <param name="options"></param>
        public static void AddIIoTHubSwaggerEndpoint(this SwaggerUIOptions options)
        {
            options.SwaggerEndpoint($"/swagger/iiot-hub-v1/swagger.json", "iiot-hub-v1");
        }
    }
}
