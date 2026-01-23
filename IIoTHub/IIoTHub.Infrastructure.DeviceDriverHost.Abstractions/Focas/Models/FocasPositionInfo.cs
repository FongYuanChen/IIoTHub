namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models
{
    /// <summary>
    /// FOCAS 位置資訊
    /// </summary>
    public class FocasPositionInfo
    {
        /// <summary>
        /// 機械座標位置
        /// </summary>
        public Dictionary<string, double> MachinePosition { get; set; }

        /// <summary>
        /// 絕對座標位置
        /// </summary>
        public Dictionary<string, double> AbsolutePosition { get; set; }

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
