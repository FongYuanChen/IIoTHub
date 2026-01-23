namespace IIoTHub.Domain.Models.DeviceSettings
{
    /// <summary>
    /// 設備驅動器參數設定項目
    /// </summary>
    public class DeviceDriverParameterSetting
    {
        public DeviceDriverParameterSetting(string key,
                                            string displayName,
                                            string note,
                                            string value)
        {
            Key = key;
            DisplayName = displayName;
            Note = note;
            Value = value;
        }

        /// <summary>
        /// 參數設定唯一識別欄位
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 參數設定顯示名稱
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// 參數設定說明
        /// </summary>
        public string Note { get; }

        /// <summary>
        /// 參數設定值
        /// </summary>
        public string Value { get; }
    }
}
