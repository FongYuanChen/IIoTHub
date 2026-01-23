namespace IIoTHub.Application.Enums
{
    /// <summary>
    /// 停止監控理由類型
    /// </summary>
    public enum StopMonitorReason
    {
        /// <summary>
        /// 暫時停止
        /// </summary>
        Temporary = 1,

        /// <summary>
        /// 設備被刪除
        /// </summary>
        Removed = 2
    }
}
