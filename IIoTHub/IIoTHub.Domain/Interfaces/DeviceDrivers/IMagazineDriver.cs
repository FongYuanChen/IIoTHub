using IIoTHub.Domain.Models.DeviceSettings;
using IIoTHub.Domain.Models.DeviceSnapshots;

namespace IIoTHub.Domain.Interfaces.DeviceDrivers
{
    /// <summary>
    /// 料倉驅動器介面
    /// </summary>
    public interface IMagazineDriver : IDeviceDriver
    {
        /// <summary>
        /// 取得指定設備的快照
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        MagazineSnapshot GetSnapshot(DeviceSetting setting);
    }
}
