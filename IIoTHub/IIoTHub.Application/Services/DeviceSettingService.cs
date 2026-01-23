using IIoTHub.Application.Enums;
using IIoTHub.Application.Interfaces;
using IIoTHub.Application.Models;
using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Models.DeviceSettings;

namespace IIoTHub.Application.Services
{
    public class DeviceSettingService : IDeviceSettingService
    {
        private readonly IDeviceSettingRepository _deviceSettingRepository;

        public DeviceSettingService(IDeviceSettingRepository deviceSettingRepository)
        {
            _deviceSettingRepository = deviceSettingRepository;
        }

        /// <summary>
        /// 設備設定變更事件
        /// </summary>
        public event EventHandler<DeviceSettingChangedEventArgs> DeviceSettingChanged;

        /// <summary>
        /// 新增設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        public async Task AddAsync(DeviceSetting deviceSetting)
        {
            await _deviceSettingRepository.AddAsync(deviceSetting);
            var eventArgs = new DeviceSettingChangedEventArgs(DeviceSettingChangeType.Added, deviceSetting);
            DeviceSettingChanged?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// 更新設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        public async Task UpdateAsync(DeviceSetting deviceSetting)
        {
            await _deviceSettingRepository.UpdateAsync(deviceSetting);
            var eventArgs = new DeviceSettingChangedEventArgs(DeviceSettingChangeType.Updated, deviceSetting);
            DeviceSettingChanged?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// 刪除指定的設備設定
        /// </summary>
        /// <param name="deviceId"></param>
        public async Task DeleteAsync(Guid deviceId)
        {
            var deviceSetting = await _deviceSettingRepository.GetByIdAsync(deviceId);
            if (deviceSetting != null)
            {
                await _deviceSettingRepository.DeleteAsync(deviceId);
                var eventArgs = new DeviceSettingChangedEventArgs(DeviceSettingChangeType.Deleted, deviceSetting);
                DeviceSettingChanged?.Invoke(this, eventArgs);
            }
        }

        /// <summary>
        /// 取得所有設備設定
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DeviceSetting>> GetAllAsync()
        {
            return await _deviceSettingRepository.GetAllAsync();
        }

        /// <summary>
        /// 取得指定的設備設定
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<DeviceSetting> GetByIdAsync(Guid deviceId)
        {
            return await _deviceSettingRepository.GetByIdAsync(deviceId);
        }
    }
}
