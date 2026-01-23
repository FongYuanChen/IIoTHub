namespace IIoTHub.Domain.Interfaces.DeviceDrivers
{
    /// <summary>
    /// 驅動器介面
    /// </summary>
    public interface IDeviceDriver
    {
        /// <summary>
        /// 驅動器名稱
        /// </summary>
        string Name { get; }
    }
}
