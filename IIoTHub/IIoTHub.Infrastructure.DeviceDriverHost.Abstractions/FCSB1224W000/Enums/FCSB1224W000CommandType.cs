namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Enums
{
    /// <summary>
    /// FCSB1224W000 指令類型
    /// </summary>
    public enum FCSB1224W000CommandType
    {
        /// <summary>
        /// 取得狀態資訊
        /// </summary>
        GetStateInfo = 1,

        /// <summary>
        /// 取得警報資訊
        /// </summary>
        GetAlarmInfo = 2,

        /// <summary>
        /// 取得程式資訊
        /// </summary>
        GetProgramInfo = 3,

        /// <summary>
        /// 取得主軸資訊
        /// </summary>
        GetSpindleInfo = 4,

        /// <summary>
        /// 取得位置資訊
        /// </summary>
        GetPositionInfo = 5
    }
}
