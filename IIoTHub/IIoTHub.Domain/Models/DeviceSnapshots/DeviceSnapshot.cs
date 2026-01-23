using IIoTHub.Domain.Enums;

namespace IIoTHub.Domain.Models.DeviceSnapshots
{
    /// <summary>
    /// 設備狀態快照
    /// </summary>
    public abstract class DeviceSnapshot
    {
        public DeviceSnapshot(Guid id,
                              DeviceRunStatus runStatus)
        {
            Id = id;
            RunStatus = runStatus;
        }

        /// <summary>
        /// 設備識別碼
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 設備當下的運行狀態
        /// </summary>
        public DeviceRunStatus RunStatus { get; }

        /// <summary>
        /// 快照產生時間
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.Now;
    }
}
