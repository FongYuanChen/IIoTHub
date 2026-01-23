using IIoTHub.Domain.Enums;

namespace IIoTHub.Domain.Models.DeviceSettings
{
    /// <summary>
    /// 設備設定資訊
    /// </summary>
    public class DeviceSetting
    {
        public DeviceSetting(Guid id,
                             string name,
                             string imageBase64String,
                             DeviceCategoryType categoryType,
                             DeviceDriverSetting driverSetting)
        {
            Id = id;
            Name = name;
            ImageBase64String = imageBase64String;
            CategoryType = categoryType;
            DriverSetting = driverSetting;
        }

        /// <summary>
        /// 設備唯一識別碼
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 設備名稱
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 設備圖示（Base64 編碼字串）
        /// </summary>
        public string ImageBase64String { get; }

        /// <summary>
        /// 設備分類類型
        /// </summary>
        public DeviceCategoryType CategoryType { get; }

        /// <summary>
        /// 設備驅動器設定
        /// </summary>
        public DeviceDriverSetting DriverSetting { get; }
    }
}
