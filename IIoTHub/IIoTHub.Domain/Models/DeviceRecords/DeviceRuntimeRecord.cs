using IIoTHub.Domain.Enums;

namespace IIoTHub.Domain.Models.DeviceRecords
{
    /// <summary>
    /// 設備運行狀態歷史紀錄
    /// </summary>
    public class DeviceRuntimeRecord
    {
        public DeviceRuntimeRecord(Guid id,
                                   DeviceRunStatus runStatus,
                                   DateTime startTime,
                                   DateTime endTime)
        {
            Id = id;
            RunStatus = runStatus;
            StartTime = startTime;
            EndTime = endTime;
        }

        /// <summary>
        /// 設備識別碼
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 設備運行狀態
        /// </summary>
        public DeviceRunStatus RunStatus { get; }

        /// <summary>
        /// 狀態開始時間
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// 狀態結束時間
        /// </summary>
        public DateTime EndTime { get; }

        /// <summary>
        /// 狀態持續時間
        /// </summary>
        public TimeSpan RunDuration => (EndTime - StartTime).Duration();
    }
}
