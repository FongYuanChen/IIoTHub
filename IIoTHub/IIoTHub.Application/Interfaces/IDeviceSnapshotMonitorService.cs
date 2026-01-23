using IIoTHub.Application.Enums;

namespace IIoTHub.Application.Interfaces
{
    /// <summary>
    /// 設備監控服務介面
    /// </summary>
    public interface IDeviceSnapshotMonitorService
    {
        /// <summary>
        /// 啟動指定設備的監控
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task StartMonitorAsync(Guid deviceId);

        /// <summary>
        /// 停止指定設備的監控
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        Task StopMonitorAsync(Guid deviceId, StopMonitorReason reason);

        /// <summary>
        /// 檢查指定設備是否正在監控中
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<bool> IsMonitoringAsync(Guid deviceId);
    }
}
