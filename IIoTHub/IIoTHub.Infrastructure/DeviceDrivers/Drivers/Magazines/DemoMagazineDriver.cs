using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.DeviceDrivers;
using IIoTHub.Domain.Models.DeviceSettings;
using IIoTHub.Domain.Models.DeviceSnapshots;
using IIoTHub.Infrastructure.DeviceDrivers.Attributes;
using System.Collections.Concurrent;

namespace IIoTHub.Infrastructure.DeviceDrivers.Drivers.Magazines
{
    public class DemoMagazineDriver : IMagazineDriver
    {
        public string Name => "DEMO 料倉專用驅動器";

        #region 參數設定

        [ParameterSetting("快照變更間隔", "單位: 秒", "60")]
        public int SnapshotChangeInterval { get; set; } = 60;

        #endregion

        #region 實作

        private readonly ConcurrentDictionary<Guid, MagazineSnapshot> _currentMagazineSnapshot = new();
        private readonly ConcurrentDictionary<Guid, DateTime> _lastUpdate = new();
        private readonly Random _random = Random.Shared;

        /// <summary>
        /// 取得指定設備的快照
        /// </summary>
        /// <param name="deviceSetting"></param>
        /// <returns></returns>
        public MagazineSnapshot GetSnapshot(DeviceSetting deviceSetting)
        {
            var parameters = deviceSetting.DriverSetting.ParameterSettings
                    .ToDictionary(v => v.Key, v => v.Value);
            var interval = GetSnapshotChangeInterval(parameters);
            var now = DateTime.Now;
            return _currentMagazineSnapshot.AddOrUpdate(
                deviceSetting.Id,
                id =>
                {
                    _lastUpdate[id] = now;
                    return RandomMagazineSnapshot(id);
                },
                (id, existingSnapshot) =>
                {
                    if (!_lastUpdate.TryGetValue(id, out var lastTime) || (now - lastTime).TotalSeconds >= interval)
                    {
                        _lastUpdate[id] = now;
                        return RandomMagazineSnapshot(id);
                    }

                    return existingSnapshot;
                });
        }

        /// <summary>
        /// 隨機產生一個料倉狀態快照
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        private MagazineSnapshot RandomMagazineSnapshot(Guid deviceId)
        {
            return new MagazineSnapshot(
                deviceId,
                RandomStatus());
        }

        /// <summary>
        /// 隨機產生一個設備狀態
        /// </summary>
        /// <returns></returns>
        private DeviceRunStatus RandomStatus()
        {
            var values = Enum.GetValues(typeof(DeviceRunStatus));
            return (DeviceRunStatus)values.GetValue(_random.Next(values.Length));
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
