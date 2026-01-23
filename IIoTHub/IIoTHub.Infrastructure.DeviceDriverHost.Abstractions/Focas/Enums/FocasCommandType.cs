namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Enums
{
    /// <summary>
    /// FOCAS 指令類型
    /// </summary>
    public enum FocasCommandType
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
