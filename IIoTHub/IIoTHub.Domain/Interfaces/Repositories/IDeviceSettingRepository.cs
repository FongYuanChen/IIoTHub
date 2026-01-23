using IIoTHub.Domain.Models.DeviceSettings;

namespace IIoTHub.Domain.Interfaces.Repositories
{
    /// <summary>
    /// 提供新增、更新、刪除、查詢等操作的設備設定Repository介面
    /// </summary>
    public interface IDeviceSettingRepository
    {
        /// <summary>
        /// 新增一筆設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        /// <returns></returns>
        Task AddAsync(DeviceSetting deviceSetting);

        /// <summary>
        /// 更新指定的設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        /// <returns></returns>
        Task UpdateAsync(DeviceSetting deviceSetting);

        /// <summary>
        /// 刪除指定的設備設定
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid deviceId);

        /// <summary>
        /// 取得所有的設備設定
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
