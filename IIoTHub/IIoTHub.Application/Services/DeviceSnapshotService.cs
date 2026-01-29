using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Models.DeviceSnapshots;

namespace IIoTHub.Application.Services
{
    public class DeviceSnapshotService : IDeviceSnapshotService
    {
        private readonly IDeviceSettingRepository _deviceSettingRepository;
        private readonly IDeviceDriverProvider _deviceDriverProvider;

        public DeviceSnapshotService(IDeviceSettingRepository deviceSettingRepository,
                                     IDeviceDriverProvider deviceDriverProvider)
        {
            _deviceSettingRepository = deviceSettingRepository;
            _deviceDriverProvider = deviceDriverProvider;
        }

        /// <summary>
        /// 取得指定機台的狀態快照
        /// </summary>
        /// <param name="machineId"></param>
        /// <returns></returns>
        public async Task<MachineSnapshot> GetMachineSnapshotAsync(Guid machineId)
        {
            var deviceSetting = await _deviceSettingRepository.GetByIdAsync(machineId);
            if (deviceSetting != null && deviceSetting.CategoryType == DeviceCategoryType.Machine)
            {
                return _deviceDriverProvider.GetMachineDriver(deviceSetting.DriverSetting.Name)
                                            .GetSnapshot(deviceSetting);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 取得指定料倉的狀態快照
        /// </summary>
        /// <param name="magazineId"></param>
        /// <returns></returns>
        public async Task<MagazineSnapshot> GetMagazineSnapshotAsync(Guid magazineId)
        {
            var deviceSetting = await _deviceSettingRepository.GetByIdAsync(magazineId);
            if (deviceSetting != null && deviceSetting.CategoryType == DeviceCategoryType.Magazine)
            {
                return _deviceDriverProvider.GetMagazineDriver(deviceSetting.DriverSetting.Name)
                                            .GetSnapshot(deviceSetting);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 取得指定機械手的狀態快照
        /// </summary>
        /// <param name="robotId"></param>
        /// <returns></returns>
        public async Task<RobotSnapshot> GetRobotSnapshotAsync(Guid robotId)
        {
            var deviceSetting = await _deviceSettingRepository.GetByIdAsync(robotId);
            if (deviceSetting != null && deviceSetting.CategoryType == DeviceCategoryType.Robot)
            {
                return _deviceDriverProvider.GetRobotDriver(deviceSetting.DriverSetting.Name)
                                            .GetSnapshot(deviceSetting);
            }
            else
            {
                return default;
            }
        }
    }
}
