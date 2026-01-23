using IIoTHub.Domain.Models.DeviceMonitor;

namespace IIoTHub.Domain.Interfaces.Repositories
{
    /// <summary>
    /// 提供新增、更新、刪除、查詢等操作的設備監控狀態Repository介面
    /// </summary>
    public interface IDeviceSnapshotMonitorStatusRepository
    {
        /// <summary>
        /// 新增一筆設備監控狀態
        /// </summary>
        /// <param name="deviceMonitorStatus"></param>
        /// <returns></returns>
        Task AddAsync(DeviceMonitorStatus deviceMonitorStatus);

        /// <summary>
        /// 更新指定的設備監控狀態
        /// </summary>
        /// <param name="deviceMonitorStatus"></param>
        /// <returns></returns>
        Task UpdateAsync(DeviceMonitorStatus deviceMonitorStatus);

        /// <summary>
        /// 刪除指定的設備監控狀態
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid deviceId);

        /// <summary>
        /// 取得所有的設備監控狀態
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DeviceMonitorStatus>> GetAllAsync();

        /// <summary>
        /// 取得指定的設備監控狀態
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<DeviceMonitorStatus> GetByIdAsync(Guid deviceId);
    }
}
