using IIoTHub.Domain.Enums;

namespace IIoTHub.Domain.Models
{
    /// <summary>
    /// 設備運行狀態歷史紀錄
    /// </summary>
    public class DeviceRuntimeRecord
    {
        public DeviceRuntimeRecord(Guid deviceId,
                                   DeviceRunStatus runStatus,
                                   DateTime startTime,
                                   DateTime endTime)
        {
            DeviceId = deviceId;
            RunStatus = runStatus;
            StartTime = startTime;
            EndTime = endTime;
        }

        /// <summary>
        /// 設備識別碼
        /// </summary>
        public Guid DeviceId { get; }

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
    }
}
