using IIoTHub.Domain.Models.DeviceSnapshots;

namespace IIoTHub.Application.Interfaces
{
    /// <summary>
    /// 設備快照服務介面
    /// </summary>
    public interface IDeviceSnapshotService
    {
        /// <summary>
        ///  取得指定機台的狀態快照
        /// </summary>
        /// <param name="machineId"></param>
        /// <returns></returns>
        Task<MachineSnapshot> GetMachineSnapshotAsync(Guid machineId);

        /// <summary>
        /// 取得指定料倉的狀態快照
        /// </summary>
        /// <param name="magazineId"></param>
        /// <returns></returns>
        Task<MagazineSnapshot> GetMagazineSnapshotAsync(Guid magazineId);

        /// <summary>
        /// 取得指定機械手的狀態快照
        /// </summary>
        /// <param name="robotId"></param>
        /// <returns></returns>
        Task<RobotSnapshot> GetRobotSnapshotAsync(Guid robotId);
    }
}
