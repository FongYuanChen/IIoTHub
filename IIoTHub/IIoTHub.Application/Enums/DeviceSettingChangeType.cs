namespace IIoTHub.Application.Enums
{
    /// <summary>
    /// 設備設定變更的類型
    /// </summary>
    public enum DeviceSettingChangeType
    {
        /// <summary>
        /// 新增一筆設備設定
        /// </summary>
        Added = 1,

        /// <summary>
        /// 更新既有的設備設定內容
        /// </summary>
        Updated = 2,

        /// <summary>
        /// 刪除既有的設備設定
        /// </summary>
        Deleted = 3
    }
}
