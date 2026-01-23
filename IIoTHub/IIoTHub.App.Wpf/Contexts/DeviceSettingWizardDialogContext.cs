using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Models.DeviceSettings;

namespace IIoTHub.App.Wpf.Contexts
{
    /// <summary>
    /// 用於 Device Setting 精靈對話框的上下文資料
    /// </summary>
    public class DeviceSettingWizardDialogContext
    {
        /// <summary>
        /// 設備唯一識別 ID
        /// </summary>
        public Guid DeviceId { get; private set; }

        /// <summary>
        /// 設備名稱
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 設備圖片的 Base64 編碼字串
        /// </summary>
        public string DeviceImageBase64String { get; set; }

        /// <summary>
        /// 設備類別
        /// </summary>
        public DeviceCategoryType DeviceCategoryType { get; set; }

        /// <summary>
        /// 設備所使用的驅動器名稱
        /// </summary>
        public string DeviceDriver { get; set; }

        /// <summary>
        /// 設備參數設定狀態列表
        /// </summary>
        public List<DeviceDriverParameterSettingState> ParameterSettings { get; set; }

        /// <summary>
        /// 重置與 Driver 相關的設定內容
        /// </summary>
        public void ResetDeviceDriverSettings()
        {
            ParameterSettings = null;
        }

        /// <summary>
        /// 建立一個新的上下文 (新增設備)
        /// </summary>
        public static DeviceSettingWizardDialogContext CreateNew()
        {
            return new DeviceSettingWizardDialogContext
            {
                DeviceId = Guid.NewGuid()
            };
        }

        /// <summary>
        /// 從現有的 DeviceSetting 建立上下文 (編輯設備)
        /// </summary>
        public static DeviceSettingWizardDialogContext FromDeviceSetting(DeviceSetting setting)
        {
            return new DeviceSettingWizardDialogContext
            {
                DeviceId = setting.Id,
                DeviceName = setting.Name,
                DeviceImageBase64String = setting.ImageBase64String,
                DeviceCategoryType = setting.CategoryType,
                DeviceDriver = setting.DriverSetting.Name,
                ParameterSettings = setting.DriverSetting.ParameterSettings.Select(parameterSetting => new DeviceDriverParameterSettingState(parameterSetting)).ToList()
            };
        }

        /// <summary>
        /// 將上下文轉換回 DeviceSetting
        /// </summary>
        /// <returns></returns>
        public DeviceSetting ConvertToDeviceSetting()
            => new DeviceSetting(DeviceId,
                                 DeviceName,
                                 DeviceImageBase64String,
                                 DeviceCategoryType,
                                 new DeviceDriverSetting(
                                     DeviceDriver,
                                     ParameterSettings.Select(setting => setting.ConvertToDeviceDriverParameterSetting()).ToList()));
    }

    /// <summary>
    /// DeviceDriverParameterSetting 在 Wizard 中的狀態封裝 (支援編輯時的暫存與轉換)
    /// </summary>
    public class DeviceDriverParameterSettingState
    {
        public DeviceDriverParameterSettingState(DeviceDriverParameterSetting parameterSetting)
        {
            Key = parameterSetting.Key;
            DisplayName = parameterSetting.DisplayName;
            Note = parameterSetting.Note;
            Value = parameterSetting.Value;
        }

        /// <summary>
        /// 參數設定的唯一 Key
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
        /// 參數設定的值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 轉換回 DeviceDriverParameterSetting
        /// </summary>
        /// <returns></returns>
        public DeviceDriverParameterSetting ConvertToDeviceDriverParameterSetting()
            => new DeviceDriverParameterSetting(Key, DisplayName, Note, Value);
    }
}
