using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Interfaces.DeviceDrivers;

namespace IIoTHub.Infrastructure.DeviceDrivers.Providers
{
    public class DeviceDriverProvider : IDeviceDriverProvider
    {
        private readonly IEnumerable<IMachineDriver> _machineDrivers;
        private readonly IEnumerable<IMagazineDriver> _magazineDrivers;
        private readonly IEnumerable<IRobotDriver> _robotDrivers;

        public DeviceDriverProvider(IEnumerable<IMachineDriver> machineDrivers,
                                    IEnumerable<IMagazineDriver> magazineDrivers,
                                    IEnumerable<IRobotDriver> robotDrivers)
        {
            _machineDrivers = machineDrivers;
            _magazineDrivers = magazineDrivers;
            _robotDrivers = robotDrivers;
        }

        /// <summary>
        /// 取得所有機台驅動器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMachineDriver> GetMachineDrivers() => _machineDrivers;

        /// <summary>
        /// 取得指定的機台驅動器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMachineDriver GetMachineDriver(string name) => _machineDrivers.FirstOrDefault(driver => driver.Name == name);

        /// <summary>
        /// 取得所有料倉驅動器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMagazineDriver> GetMagazineDrivers() => _magazineDrivers;

        /// <summary>
        /// 取得指定的料倉驅動器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMagazineDriver GetMagazineDriver(string name) => _magazineDrivers.FirstOrDefault(driver => driver.Name == name);

        /// <summary>
        /// 取得所有機械手驅動器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IRobotDriver> GetRobotDrivers() => _robotDrivers;

        /// <summary>
        /// 取得指定的機械手驅動器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IRobotDriver GetRobotDriver(string name) => _robotDrivers.FirstOrDefault(driver => driver.Name == name);
    }
}
