namespace IIoTHub.Infrastructure.DeviceDrivers.Attributes
{
    /// <summary>
    /// 標記為參數設定的屬性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterSettingAttribute : Attribute
    {
        public ParameterSettingAttribute(string displayName,
                                         string note = "",
                                         string defaultValue = "")
        {
            DisplayName = displayName;
            Note = note;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// 設定項在 UI 或文件中顯示的名稱
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// 對該設定項的說明或備註
        /// </summary>
        public string Note { get; }

        /// <summary>
        /// 設定項的預設值
        /// </summary>
        public string DefaultValue { get; }
    }
}
