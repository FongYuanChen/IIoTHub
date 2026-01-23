using IIoTHub.Domain.Enums;

namespace IIoTHub.Domain.Models.DeviceSnapshots
{
    /// <summary>
    /// 機械手狀態快照
    /// </summary>
    public class RobotSnapshot : DeviceSnapshot
    {
        public RobotSnapshot(Guid id,
                             DeviceRunStatus runStatus) : base(id, runStatus)
        {

        }

        /// <summary>
        /// 離線快照
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static RobotSnapshot OfflineSnapshot(Guid id)
        {
            return new RobotSnapshot(id, DeviceRunStatus.Offline);
        }
    }
}
