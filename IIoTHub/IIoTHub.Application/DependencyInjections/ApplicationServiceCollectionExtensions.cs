using IIoTHub.Application.Interfaces;
using IIoTHub.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IIoTHub.Application.DependencyInjections
{
    /// <summary>
    /// 擴充方法，提供將應用層服務註冊到依賴注入容器的功能
    /// </summary>
    public static class ApplicationServiceCollectionExtensions
    {
        /// <summary>
        /// 註冊應用層服務
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<IDeviceSnapshotService, DeviceSnapshotService>();
            services.AddSingleton<IDeviceSnapshotMonitorService, DeviceSnapshotMonitorService>();
            services.AddSingleton<IDeviceSnapshotPublisher, DeviceSnapshotPublisher>();
            services.AddSingleton<IDeviceRuntimeStatisticsService, DeviceRuntimeStatisticsService>();
            services.AddSingleton<IDeviceSettingService, DeviceSettingService>();
            return services;
        }
    }
}
