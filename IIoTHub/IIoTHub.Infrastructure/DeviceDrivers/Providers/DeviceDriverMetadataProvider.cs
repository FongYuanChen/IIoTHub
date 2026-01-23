using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.DeviceDrivers;
using IIoTHub.Domain.Models.DeviceSettings;
using IIoTHub.Infrastructure.DeviceDrivers.Attributes;
using System.Reflection;

namespace IIoTHub.Infrastructure.DeviceDrivers.Providers
{
    public class DeviceDriverMetadataProvider : IDeviceDriverMetadataProvider
    {
        private readonly IDeviceDriverProvider _deviceDriverProvider;

        public DeviceDriverMetadataProvider(IDeviceDriverProvider deviceDriverProvider)
        {
            _deviceDriverProvider = deviceDriverProvider;
        }

        /// <summary>
        /// 取得指定設備類別的驅動器資料列表
        /// </summary>
        /// <param name="categoryType"></param>
        /// <returns></returns>
        public IEnumerable<DeviceDriverSetting> GetDriverMetadata(DeviceCategoryType categoryType)
            => categoryType switch
            {
                DeviceCategoryType.Machine
                    => BuildDriverSettings(_deviceDriverProvider.GetMachineDrivers()),

                DeviceCategoryType.Magazine
                    => BuildDriverSettings(_deviceDriverProvider.GetMagazineDrivers()),

                DeviceCategoryType.Robot
                    => BuildDriverSettings(_deviceDriverProvider.GetRobotDrivers()),

                _ => []
            };

        /// <summary>
        /// 將設備驅動器列表轉換為設備驅動器設定列表
        /// </summary>
        /// <typeparam name="TDriver"></typeparam>
        /// <param name="drivers"></param>
        /// <returns></returns>
        private static IEnumerable<DeviceDriverSetting> BuildDriverSettings(IEnumerable<IDeviceDriver> drivers)
            => drivers.Select(driver =>
                new DeviceDriverSetting(
                    driver.Name,
                    GetSettings<ParameterSettingAttribute>(driver)
                        .Select(settings => new DeviceDriverParameterSetting(
                            settings.Property.Name,
                            settings.Attribute.DisplayName,
                            settings.Attribute.Note,
                            settings.Attribute.DefaultValue))
                        .ToList()
                )
            );

        /// <summary>
        /// 取得驅動器上標記特定 Attribute 的屬性與對應 Attribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="driver"></param>
        /// <returns></returns>
        private static IEnumerable<(PropertyInfo Property, TAttribute Attribute)> GetSettings<TAttribute>(object driver) where TAttribute : Attribute
            => driver.GetType()
                     .GetProperties()
                     .Select(p => (Property: p, Attribute: p.GetCustomAttribute<TAttribute>()))
                     .Where(t => t.Attribute != null);
    }
}
