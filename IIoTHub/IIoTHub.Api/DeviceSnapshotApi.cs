using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Models.DeviceSnapshots;
using Microsoft.AspNetCore.Mvc;

namespace IIoTHub.Api
{
    /// <summary>
    /// 設備狀態快照相關接口
    /// </summary>
    [Route("api/deviceSnapshot")]
    [ApiController]
    public class DeviceSnapshotApi : ControllerBase
    {
        private readonly IDeviceSnapshotService _deviceSnapshotService;

        public DeviceSnapshotApi(IDeviceSnapshotService deviceSnapshotService)
        {
            _deviceSnapshotService = deviceSnapshotService;
        }

        /// <summary>
        ///  取得指定機台的狀態快照
        /// </summary>
        /// <param name="machineId"></param>
        /// <returns></returns>
        [HttpGet("machine/{machineId}")]
        public async Task<MachineSnapshot> GetMachineSnapshotAsync(Guid machineId)
        {
            return await _deviceSnapshotService.GetMachineSnapshotAsync(machineId);
        }

        /// <summary>
        /// 取得指定料倉的狀態快照
        /// </summary>
        /// <param name="magazineId"></param>
        /// <returns></returns>
        [HttpGet("magazine/{magazineId}")]
        public async Task<MagazineSnapshot> GetMagazineSnapshotAsync(Guid magazineId)
        {
            return await _deviceSnapshotService.GetMagazineSnapshotAsync(magazineId);
        }

        /// <summary>
        /// 取得指定機械手的狀態快照
        /// </summary>
        /// <param name="robotId"></param>
        /// <returns></returns>
        [HttpGet("robot/{robotId}")]
        public async Task<RobotSnapshot> GetRobotSnapshotAsync(Guid robotId)
        {
            return await _deviceSnapshotService.GetRobotSnapshotAsync(robotId);
        }
    }
}
