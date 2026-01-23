using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Interfaces.DeviceDrivers;
using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Infrastructure.DeviceDriverHosts;
using IIoTHub.Infrastructure.DeviceDriverHosts.Interfaces;
using IIoTHub.Infrastructure.DeviceDrivers.Providers;
using IIoTHub.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IIoTHub.Infrastructure.DependencyInjections
{
    /// <summary>
    /// 擴充方法，提供將基礎建設層服務註冊到依賴注入容器的功能
    /// </summary>
    public static class InfrastructureServiceCollectionExtensions
    {
        /// <summary>
        /// 註冊基礎建設層服務
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IDeviceDriverHostManager, DeviceDriverHostManager>();
            services.AddDrivers<IMachineDriver>();
            services.AddDrivers<IMagazineDriver>();
            services.AddDrivers<IRobotDriver>();
            services.AddSingleton<IDeviceDriverMetadataProvider, DeviceDriverMetadataProvider>();
            services.AddSingleton<IDeviceDriverProvider, DeviceDriverProvider>();
            services.AddSingleton<IDeviceSnapshotMonitorStatusRepository, JsonDeviceSnapshotMonitorStatusRepository>();
            services.AddSingleton<IDeviceSettingRepository, JsonDeviceSettingRepository>();
            services.AddSingleton<IDeviceRuntimeRepository, SqliteDeviceRuntimeRepository>();
            return services;
        }

        /// <summary>
        /// 掃描 AppDomain 中所有非抽象類別，並將符合指定介面 TDriver 的實作註冊為 Singleton
        /// </summary>
        /// <typeparam name="TDriver"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddDrivers<TDriver>(this IServiceCollection services)
        {
            var interfaceType = typeof(TDriver);
            var instanceTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(type => type != null)!;
                    }
                })
                .Where(type => !type.IsInterface && !type.IsAbstract)
                .Where(type => interfaceType.IsAssignableFrom(type));
            foreach (var instanceType in instanceTypes)
            {
                services.AddSingleton(interfaceType, instanceType);
            }
            return services;
        }
    }
}
