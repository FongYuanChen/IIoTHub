using IIoTHub.Domain.Interfaces.DeviceDrivers;

namespace IIoTHub.Application.Interfaces
{
    /// <summary>
    /// 提供設備驅動器的介面
    /// </summary>
    public interface IDeviceDriverProvider
    {
        /// <summary>
        /// 取得所有機台驅動器
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMachineDriver> GetMachineDrivers();

        /// <summary>
        /// 取得指定的機台驅動器
        /// </summary>
        /// <param name="driverName"></param>
        /// <returns></returns>
        IMachineDriver GetMachineDriver(string driverName);

        /// <summary>
        /// 取得所有料倉驅動器
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMagazineDriver> GetMagazineDrivers();

        /// <summary>
        /// 取得指定的料倉驅動器
        /// </summary>
        /// <param name="driverName"></param>
        /// <returns></returns>
        IMagazineDriver GetMagazineDriver(string driverName);

        /// <summary>
        /// 取得所有機械手驅動器
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRobotDriver> GetRobotDrivers();

        /// <summary>
        /// 取得指定的機械手驅動器
        /// </summary>
        /// <param name="driverName"></param>
        /// <returns></returns>
        IRobotDriver GetRobotDriver(string driverName);
    }
}
