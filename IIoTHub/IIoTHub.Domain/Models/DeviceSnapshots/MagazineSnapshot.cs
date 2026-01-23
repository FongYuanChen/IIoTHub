using IIoTHub.Domain.Enums;

namespace IIoTHub.Domain.Models.DeviceSnapshots
{
    /// <summary>
    /// 料倉狀態快照
    /// </summary>
    public class MagazineSnapshot : DeviceSnapshot
    {
        public MagazineSnapshot(Guid id,
                                DeviceRunStatus runStatus) : base(id, runStatus)
        {
        }

        /// <summary>
        /// 離線快照
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MagazineSnapshot OfflineSnapshot(Guid id)
        {
            return new MagazineSnapshot(id, DeviceRunStatus.Offline);
        }
    }
}
