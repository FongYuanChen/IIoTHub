namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Models
{
    /// <summary>
    /// FCSB1224W000 警報資訊
    /// </summary>
    public class FCSB1224W000AlarmInfo
    {
        /// <summary>
        /// 警報訊息清單
        /// </summary>
        public List<FCSB1224W000AlarmMessage> AlarmMessages { get; set; }
    }

    /// <summary>
    /// FCSB1224W000 警報訊息
    /// </summary>
    public class FCSB1224W000AlarmMessage
    {
        /// <summary>
        /// 警報編號
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// 警報訊息內容
        /// </summary>
        public string Message { get; set; }
    }
}
