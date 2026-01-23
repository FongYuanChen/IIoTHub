using IIoTHub.Domain.Models.DeviceRecords;

namespace IIoTHub.Application.Models
{
    /// <summary>
    /// 設備運轉統計摘變化事件的事件參數
    /// </summary>
    public class DeviceRuntimeSummaryChangedEventArgs : EventArgs
    {
        public DeviceRuntimeSummaryChangedEventArgs(DeviceRuntimeSummary deviceRuntimeSummary)
        {
            DeviceRuntimeSummary = deviceRuntimeSummary;
        }

        /// <summary>
        /// 變更的設備運轉統計摘要
        /// </summary>
        public DeviceRuntimeSummary DeviceRuntimeSummary { get; }
    }

    /// <summary>
    /// 設備運轉統計摘要
    /// </summary>
    public class DeviceRuntimeSummary
    {
        /// <summary>
        /// 設備運轉時間軸紀錄列表
        /// </summary>
        public List<DeviceRuntimeRecord> RuntimeRecords { get; set; } = new();

        /// <summary>
        /// 離線所佔的時間比例
        /// </summary>
        public double OfflineUtilization { get; set; }

        /// <summary>
        /// 待機狀態所佔的時間比例
        /// </summary>
        public double StandbyUtilization { get; set; }

        /// <summary>
        /// 運轉中所佔的時間比例
        /// </summary>
        public double RunningUtilization { get; set; }

        /// <summary>
        /// 警報所佔的時間比例
        /// </summary>
        public double AlarmUtilization { get; set; }
    }
}
