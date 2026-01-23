using IIoTHub.Application.Enums;
using IIoTHub.Domain.Models.DeviceSettings;

namespace IIoTHub.Application.Models
{
    /// <summary>
    /// 設備設定變更事件的事件參數
    /// </summary>
    public class DeviceSettingChangedEventArgs : EventArgs
    {
        public DeviceSettingChangedEventArgs(DeviceSettingChangeType changeType,
                                             DeviceSetting deviceSetting)
        {
            ChangeType = changeType;
            DeviceSetting = deviceSetting;
        }

        /// <summary>
        /// 設備設定的變更類型
        /// </summary>
        public DeviceSettingChangeType ChangeType { get; }

        /// <summary>
        /// 變更的設備設定
        /// </summary>
        public DeviceSetting DeviceSetting { get; }
    }
}
