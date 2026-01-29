namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Enums
{
    /// <summary>
    ///  FCSB1224W000 機台運轉狀態
    /// </summary>
    public enum FCSB1224W000RunStatus
    {
        /// <summary>
        /// 閒置
        /// </summary>
        Reset = 1,

        /// <summary>
        /// 停止
        /// </summary>
        Stop = 2,

        /// <summary>
        /// 暫停
        /// </summary>
        Hold = 3,

        /// <summary>
        /// 執行中
        /// </summary>
        Start = 4,

        /// <summary>
        /// 警報
        /// </summary>
        Alarm = 5
    }
}
