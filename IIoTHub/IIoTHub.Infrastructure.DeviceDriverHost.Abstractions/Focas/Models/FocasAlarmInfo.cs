namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models
{
    /// <summary>
    /// FOCAS 警報資訊
    /// </summary>
    public class FocasAlarmInfo
    {
        /// <summary>
        /// 警報訊息清單
        /// </summary>
        public List<FocasAlarmMessage> AlarmMessages { get; set; }
    }

    /// <summary>
    /// FOCAS 警報訊息
    /// </summary>
    public class FocasAlarmMessage
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
