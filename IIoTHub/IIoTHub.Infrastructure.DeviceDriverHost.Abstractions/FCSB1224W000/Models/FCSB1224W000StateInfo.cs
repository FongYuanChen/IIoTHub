using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Enums;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Models
{
    /// <summary>
    /// FCSB1224W000 狀態資訊
    /// </summary>
    public class FCSB1224W000StateInfo
    {
        /// <summary>
        /// 運轉狀態
        /// </summary>
        public FCSB1224W000RunStatus RunStatus { get; set; }

        /// <summary>
        /// 操作模式
        /// </summary>
        public string OperatingMode { get; set; }
    }
}
