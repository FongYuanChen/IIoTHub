using IIoTHub.Domain.Enums;

namespace IIoTHub.Domain.Models.DeviceSnapshots
{
    /// <summary>
    /// 機台狀態快照
    /// </summary>
    public class MachineSnapshot : DeviceSnapshot
    {
        public MachineSnapshot(Guid id,
                               DeviceRunStatus runStatus,
                               string operatingMode,
                               string mainProgramName,
                               string executingProgramName,
                               int feedRateActual,
                               int feedRatePercentage,
                               int feedRateRapidPercentage,
                               int speedActual,
                               int speedPercentage,
                               int loadPercentage,
                               Dictionary<string, double> machinePosition,
                               Dictionary<string, double> absolutePosition,
                               Dictionary<string, double> relativePosition,
                               Dictionary<string, double> distanceToGo) : base(id, runStatus)
        {
            OperatingMode = operatingMode;
            MainProgramName = mainProgramName;
            ExecutingProgramName = executingProgramName;
            FeedRateActual = feedRateActual;
            FeedRatePercentage = feedRatePercentage;
            FeedRateRapidPercentage = feedRateRapidPercentage;
            SpeedActual = speedActual;
            SpeedPercentage = speedPercentage;
            LoadPercentage = loadPercentage;
            MachinePosition = machinePosition;
            AbsolutePosition = absolutePosition;
            RelativePosition = relativePosition;
            DistanceToGo = distanceToGo;
        }

        /// <summary>
        /// 離線快照
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MachineSnapshot OfflineSnapshot(Guid id)
        {
            return new MachineSnapshot(
                id,
                DeviceRunStatus.Offline,
                default,
                default, 
                default,
                default,
                default,
                default, 
                default,
                default,
                default,
                default,
                default,
                default,
                default);
        }

        /// <summary>
        /// 操作模式
        /// </summary>
        public string OperatingMode { get; }

        /// <summary>
        /// 主程式
        /// </summary>
        public string MainProgramName { get; }

        /// <summary>
        /// 執行程式
        /// </summary>
        public string ExecutingProgramName { get; }

        /// <summary>
        /// 主軸進給
        /// </summary>
        public int FeedRateActual { get; }

        /// <summary>
        /// 主軸進給倍率
        /// </summary>
        public int FeedRatePercentage { get; }

        /// <summary>
        /// 主軸快速進給倍率
        /// </summary>
        public int FeedRateRapidPercentage { get; }

        /// <summary>
        /// 主軸轉速
        /// </summary>
        public int SpeedActual { get; }

        /// <summary>
        /// 主軸轉速倍率
        /// </summary>
        public int SpeedPercentage { get; }

        /// <summary>
        /// 主軸負載
        /// </summary>
        public int LoadPercentage { get; }

        /// <summary>
        /// 機械座標
        /// </summary>
        public IReadOnlyDictionary<string, double> MachinePosition { get; }

        /// <summary>
        /// 絕對座標
        /// </summary>
        public IReadOnlyDictionary<string, double> AbsolutePosition { get; }

        /// <summary>
        /// 相對座標
        /// </summary>
        public IReadOnlyDictionary<string, double> RelativePosition { get; }

        /// <summary>
        /// 剩餘距離
        /// </summary>
        public IReadOnlyDictionary<string, double> DistanceToGo { get; }
    }
}
