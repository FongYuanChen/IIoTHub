using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.DeviceDrivers;
using IIoTHub.Domain.Models.DeviceSettings;
using IIoTHub.Domain.Models.DeviceSnapshots;
using IIoTHub.Infrastructure.DeviceDrivers.Attributes;
using System.Collections.Concurrent;

namespace IIoTHub.Infrastructure.DeviceDrivers.Drivers.Machines
{
    public class DemoMachineDriver : IMachineDriver
    {
        public string Name => "DEMO 機台專用驅動器";

        #region 參數設定

        [ParameterSetting("快照變更間隔", "單位: 秒", "60")]
        public int SnapshotChangeInterval { get; set; } = 60;

        #endregion

        #region 實作

        private readonly ConcurrentDictionary<Guid, MachineSnapshot> _currentMachineSnapshot = new();
        private readonly ConcurrentDictionary<Guid, DateTime> _lastUpdate = new();
        private readonly Random _random = Random.Shared;
        private readonly List<string> _operatingMode = ["****", "MDI", "DNC", "MEM", "EDIT", "Teach in"];
        private readonly List<string> _programNames = ["O5000", "O5001", "O5002", "O5003", "O5004", "O5005", "O5006", "O5007", "O5008", "O5009", "O5010"];
        private readonly List<int> _feedRatePercentages = [0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200];
        private readonly List<int> _feedRateRapidPercentages = [ 0, 25, 50, 100 ];
        private readonly List<int> _speedPercentages = [ 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200 ];
        private readonly List<int> _loadPercentages = [ 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 ];

        /// <summary>
        /// 取得指定設備的快照
        /// </summary>
        /// <param name="deviceSetting"></param>
        /// <returns></returns>
        public MachineSnapshot GetSnapshot(DeviceSetting deviceSetting)
        {
            var parameters = deviceSetting.DriverSetting.ParameterSettings
                    .ToDictionary(v => v.Key, v => v.Value);
            var interval = GetSnapshotChangeInterval(parameters);
            var now = DateTime.Now;
            return _currentMachineSnapshot.AddOrUpdate(
                deviceSetting.Id,
                id =>
                {
                    _lastUpdate[id] = now;
                    return RandomMachineSnapshot(id);
                },
                (id, existingSnapshot) =>
                {
                    if (!_lastUpdate.TryGetValue(id, out var lastTime) || (now - lastTime).TotalSeconds >= interval)
                    {
                        _lastUpdate[id] = now;
                        return RandomMachineSnapshot(id);
                    }

                    return existingSnapshot;
                });
        }

        /// <summary>
        /// 隨機產生一個機台狀態快照
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        private MachineSnapshot RandomMachineSnapshot(Guid deviceId)
        {
            return new MachineSnapshot(
                deviceId,
                RandomStatus(),
                RandomOperatingMode(),
                RandomMainProgramName(),
                RandomExecutingProgramName(),
                RandomFeedRateActual(),
                RandomFeedRatePercentage(),
                RandomFeedRateRapidPercentage(),
                RandomSpeedActual(),
                RandomSpeedPercentage(),
                RandomLoadPercentage(),
                RandomPosition(),
                RandomPosition(),
                RandomPosition(),
                RandomPosition());
        }

        /// <summary>
        /// 隨機產生一個狀態
        /// </summary>
        /// <returns></returns>
        private DeviceRunStatus RandomStatus()
        {
            var values = Enum.GetValues(typeof(DeviceRunStatus));
            return (DeviceRunStatus)values.GetValue(_random.Next(values.Length));
        }

        /// <summary>
        /// 隨機產生一個操作模式
        /// </summary>
        /// <returns></returns>
        private string RandomOperatingMode()
        {
            return _operatingMode[_random.Next(_operatingMode.Count)];
        }

        /// <summary>
        /// 隨機產生一個主程式名稱
        /// </summary>
        /// <returns></returns>
        private string RandomMainProgramName()
        {
            return _programNames[0];
        }

        /// <summary>
        /// 隨機產生一個執行程式名稱
        /// </summary>
        /// <returns></returns>
        private string RandomExecutingProgramName()
        {
            return _programNames[_random.Next(_programNames.Count)];
        }

        /// <summary>
        /// 隨機產生一個主軸進給
        /// </summary>
        /// <returns></returns>
        private int RandomFeedRateActual()
        {
            return _random.Next(3000);
        }

        /// <summary>
        /// 隨機產生一個主軸進給倍率
        /// </summary>
        /// <returns></returns>
        private int RandomFeedRatePercentage()
        {
            return _feedRatePercentages[_random.Next(_feedRatePercentages.Count)];
        }

        /// <summary>
        /// 隨機產生一個主軸快速進給倍率
        /// </summary>
        /// <returns></returns>
        private int RandomFeedRateRapidPercentage()
        {
            return _feedRateRapidPercentages[_random.Next(_feedRateRapidPercentages.Count)];
        }

        /// <summary>
        /// 隨機產生一個主軸轉速
        /// </summary>
        /// <returns></returns>
        private int RandomSpeedActual()
        {
            return _random.Next(30000);
        }

        /// <summary>
        /// 隨機產生一個主軸轉速倍率
        /// </summary>
        /// <returns></returns>
        private int RandomSpeedPercentage()
        {
            return _speedPercentages[_random.Next(_speedPercentages.Count)];
        }

        /// <summary>
        /// 隨機產生一個主軸負載
        /// </summary>
        /// <returns></returns>
        private int RandomLoadPercentage()
        {
            return _loadPercentages[_random.Next(_loadPercentages.Count)];
        }

        /// <summary>
        /// 隨機產生一個座標位置
        /// </summary>
        /// <returns></returns>
        private Dictionary<string,double> RandomPosition()
        {
            return new Dictionary<string, double>
            {
                { "X", _random.NextDouble()*500},
                { "Y", _random.NextDouble()*500},
                { "Z", _random.NextDouble()*500},
                { "A", 0},
                { "B", 0},
                { "C", 0}
            };
        }

        /// <summary>
        /// 取得快照變更間隔
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private int GetSnapshotChangeInterval(Dictionary<string, string> parameters)
        {
            var interval = DriverShared.GetInt(parameters, nameof(SnapshotChangeInterval), SnapshotChangeInterval);
            return interval;
        }

        #endregion
    }
}
