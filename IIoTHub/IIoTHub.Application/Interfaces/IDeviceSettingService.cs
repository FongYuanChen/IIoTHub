using IIoTHub.Application.Models;
using IIoTHub.Domain.Models.DeviceSettings;

namespace IIoTHub.Application.Interfaces
{
    /// <summary>
    /// 設備設定服務介面
    /// </summary>
    public interface IDeviceSettingService
    {
        /// <summary>
        /// 設備設定變更事件
        /// </summary>
        event EventHandler<DeviceSettingChangedEventArgs> DeviceSettingChanged;

        /// <summary>
        /// 新增設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        Task AddAsync(DeviceSetting deviceSetting);

        /// <summary>
        /// 更新設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        Task UpdateAsync(DeviceSetting deviceSetting);

        /// <summary>
        /// 刪除指定的設備設定
        /// </summary>
        /// <param name="deviceId"></param>
        Task DeleteAsync(Guid deviceId);

        /// <summary>
        /// 取得所有設備設定
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DeviceSetting>> GetAllAsync();

        /// <summary>
        /// 取得指定的設備設定
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<DeviceSetting> GetByIdAsync(Guid deviceId);
    }
}
