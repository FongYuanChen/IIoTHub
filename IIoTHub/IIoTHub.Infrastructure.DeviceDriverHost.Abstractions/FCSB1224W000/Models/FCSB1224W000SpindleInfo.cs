namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Models
{
    /// <summary>
    /// FCSB1224W000 主軸資訊
    /// </summary>
    public class FCSB1224W000SpindleInfo
    {
        /// <summary>
        /// 主軸進給速率資訊
        /// </summary>
        public FCSB1224W000FeedRateInfo FeedRateInfo { get; set; }

        /// <summary>
        /// 主軸轉速資訊
        /// </summary>
        public FCSB1224W000SpeedInfo SpeedInfo { get; set; }

        /// <summary>
        /// 主軸負載資訊
        /// </summary>
        public FCSB1224W000LoadInfo LoadInfo { get; set; }
    }

    /// <summary>
    /// 主軸進給速率資訊
    /// </summary>
    public class FCSB1224W000FeedRateInfo
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
    public class FCSB1224W000SpeedInfo
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
    public class FCSB1224W000LoadInfo
    {
        /// <summary>
        /// 主軸負載 (%)
        /// </summary>
        public int Percentage { get; set; }
    }
}
