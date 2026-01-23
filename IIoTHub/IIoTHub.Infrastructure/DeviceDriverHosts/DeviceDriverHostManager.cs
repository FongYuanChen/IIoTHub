using IIoTHub.Infrastructure.DeviceDriverHosts.Interfaces;
using IIoTHub.Infrastructure.DeviceDriverHosts.Models;
using System.Collections.Concurrent;

namespace IIoTHub.Infrastructure.DeviceDriverHosts
{
    public class DeviceDriverHostManager : IDeviceDriverHostManager, IDisposable
    {
        private readonly ConcurrentDictionary<string, DeviceDriverHostProcess> _hostProcesses = new();

        /// <summary>
        /// 確保指定的設備驅動器宿主正在執行。
        /// 若宿主尚未啟動，會啟動它；若已啟動則維持運行。
        /// </summary>
        /// <param name="descriptor"></param>
        public void EnsureRunning(DeviceDriverHostDescriptor descriptor)
        {
            var hostProcess = _hostProcesses.GetOrAdd(descriptor.Name,
                                                      _ => new DeviceDriverHostProcess(descriptor));
            hostProcess.EnsureRunning();
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        public void Dispose()
        {
            foreach (var hostProcess in _hostProcesses.Values)
                hostProcess.Dispose();

            _hostProcesses.Clear();
        }
    }
}
