using IIoTHub.Domain.Models.DeviceSettings;
using IIoTHub.Domain.Models.DeviceSnapshots;

namespace IIoTHub.Domain.Interfaces.DeviceDrivers
{
    /// <summary>
    /// 機台驅動器介面
    /// </summary>
    public interface IMachineDriver : IDeviceDriver
    {
        /// <summary>
        /// 取得指定設備的快照
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        MachineSnapshot GetSnapshot(DeviceSetting setting);
    }
}
