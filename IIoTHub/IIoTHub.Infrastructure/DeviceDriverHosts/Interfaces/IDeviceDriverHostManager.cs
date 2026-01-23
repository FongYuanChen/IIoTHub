using IIoTHub.Infrastructure.DeviceDriverHosts.Models;

namespace IIoTHub.Infrastructure.DeviceDriverHosts.Interfaces
{
    /// <summary>
    /// 設備驅動器宿主管理介面
    /// </summary>
    public interface IDeviceDriverHostManager
    {
        /// <summary>
        /// 確保指定的設備驅動器宿主正在執行。
        /// 若宿主尚未啟動，會啟動它；若已啟動則維持運行。
        /// </summary>
        /// <param name="descriptor"></param>
        void EnsureRunning(DeviceDriverHostDescriptor descriptor);
    }
}
