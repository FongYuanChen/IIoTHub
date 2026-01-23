using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Enums;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models
{
    /// <summary>
    /// 主軸相關 PMC 讀取設定
    /// </summary>
    public class FocasSpindlePMCReadSetting
    {
        public FocasSpindlePMCReadSetting(FocasPMCReadSetting feedratePercentage,
                                          FocasPMCReadSetting feedrateRapidPercentage,
                                          FocasPMCReadSetting speedPercentage)
        {
            FeedratePercentage = feedratePercentage;
            FeedrateRapidPercentage = feedrateRapidPercentage;
            SpeedPercentage = speedPercentage;
        }

        /// <summary>
        /// 進給率百分比
        /// </summary>
        public FocasPMCReadSetting FeedratePercentage { get; }

        /// <summary>
        /// 快速進給率百分比
        /// </summary>
        public FocasPMCReadSetting FeedrateRapidPercentage { get; }

        /// <summary>
        /// 主軸轉速百分比
        /// </summary>
        public FocasPMCReadSetting SpeedPercentage { get; }
    }

    /// <summary>
    /// PMC讀取設定
    /// </summary>
    public class FocasPMCReadSetting
    {
        public FocasPMCReadSetting(FocasPMCAddressType addressType,
                                   FocasPMCAddressDataType addressDataType,
                                   int addressIndex)
        {
            AddressType = addressType;
            AddressDataType = addressDataType;
            AddressIndex = addressIndex;
        }

        /// <summary>
        /// PMC 位址型別
        /// </summary>
        public FocasPMCAddressType AddressType { get; }

        /// <summary>
        /// PMC 資料型別
        /// </summary>
        public FocasPMCAddressDataType AddressDataType { get; }

        /// <summary>
        /// PMC 位址索引
        /// </summary>
        public int AddressIndex { get; }
    }
}
