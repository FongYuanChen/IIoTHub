namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models
{
    /// <summary>
    /// FOCAS 主軸資訊
    /// </summary>
    public class FocasSpindleInfo
    {
        /// <summary>
        /// 主軸進給速率資訊
        /// </summary>
        public FocasFeedRateInfo FeedRateInfo { get; set; }

        /// <summary>
        /// 主軸轉速資訊
        /// </summary>
        public FocasSpeedInfo SpeedInfo { get; set; }

        /// <summary>
        /// 主軸負載資訊
        /// </summary>
        public FocasLoadInfo LoadInfo { get; set; }
    }

    /// <summary>
    /// 主軸進給速率資訊
    /// </summary>
    public class FocasFeedRateInfo
    {
        /// <summary>
        /// 實際進給速率 (mm/min)
        /// </summary>
        public int Actual { get; set; }

        /// <summary>
        /// 進給倍率 (%)
        /// </summary>
        public int Percentage { get; set; }

        /// <summary>
        /// 快速進給倍率 (%)
        /// </summary>
        public int RapidPercentage { get; set; }
    }

    /// <summary>
    /// 主軸轉速資訊
    /// </summary>
    public class FocasSpeedInfo
    {
        /// <summary>
        /// 實際主軸轉速 (rpm)
        /// </summary>
        public int Actual { get; set; }

        /// <summary>
        /// 主軸轉速倍率 (%)
        /// </summary>
        public int Percentage { get; set; }
    }

    /// <summary>
    /// 主軸負載資訊
    /// </summary>
    public class FocasLoadInfo
    {
        /// <summary>
        /// 主軸負載 (%)
        /// </summary>
        public int Percentage { get; set; }
    }
}
