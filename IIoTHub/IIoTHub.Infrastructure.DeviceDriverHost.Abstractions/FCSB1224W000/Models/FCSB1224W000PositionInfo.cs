namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Models
{
    /// <summary>
    /// FCSB1224W000 位置資訊
    /// </summary>
    public class FCSB1224W000PositionInfo
    {
        /// <summary>
        /// 機械座標位置
        /// </summary>
        public Dictionary<string, double> MachinePosition { get; set; }

        /// <summary>
        /// 相對座標位置
        /// </summary>
        public Dictionary<string, double> RelativePosition { get; set; }

        /// <summary>
        /// 剩餘移動距離
        /// </summary>
        public Dictionary<string, double> DistanceToGo { get; set; }
    }
}
