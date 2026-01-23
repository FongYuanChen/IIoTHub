namespace IIoTHub.Domain.Models.DeviceSettings
{
    /// <summary>
    /// 設備驅動器設定
    /// </summary>
    public class DeviceDriverSetting
    {
        public DeviceDriverSetting(string name,
                                   List<DeviceDriverParameterSetting> parameterSettings)
        {
            Name = name;
            ParameterSettings = parameterSettings ?? [];
        }

        /// <summary>
        /// 驅動器名稱 (唯一)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 驅動器參數設定項目列表
        /// </summary>
        public IReadOnlyList<DeviceDriverParameterSetting> ParameterSettings { get; }
    }
}
