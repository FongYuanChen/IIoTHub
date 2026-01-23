using IIoTHub.Application.Interfaces;
using IIoTHub.Application.Models;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Models.DeviceRecords;

namespace IIoTHub.Application.Services
{
    public class DeviceRuntimeStatisticsService : IDeviceRuntimeStatisticsService
    {
        private readonly IDeviceRuntimeRepository _deviceRuntimeRepository;

        public DeviceRuntimeStatisticsService(IDeviceRuntimeRepository deviceRuntimeRepository)
        {
            _deviceRuntimeRepository = deviceRuntimeRepository;
        }

        /// <summary>
        /// 設備運轉統計摘要變化事件
        /// </summary>
        public event EventHandler<DeviceRuntimeSummaryChangedEventArgs> DeviceRuntimeSummaryChanged;

        /// <summary>
        /// 通知服務設備的運轉狀態已發生變化。
        /// 此方法由設備監控服務呼叫。
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="runStatus"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public async Task OnRuntimeStatusChangedAsync(Guid deviceId, DeviceRunStatus runStatus, DateTime timestamp)
        {
            var last = await _deviceRuntimeRepository.GetLatestRecordAsync(deviceId);

            if (last == null || last.RunStatus != runStatus)
            {
                // 狀態變了 → 新一段
                var record = new DeviceRuntimeRecord(deviceId, runStatus, timestamp, timestamp);
                await _deviceRuntimeRepository.AddAsync(record);
            }
            else
            {
                // 狀態沒變 → 延長
                var record = new DeviceRuntimeRecord(deviceId, runStatus, last.StartTime, timestamp);
                await _deviceRuntimeRepository.UpdateAsync(record);
            }

            var eventArgs = new DeviceRuntimeSummaryChangedEventArgs(await GetDeviceRuntimeSummaryAsync(deviceId, to: timestamp));
            DeviceRuntimeSummaryChanged?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// 獲取設備運轉統計摘要
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private async Task<DeviceRuntimeSummary> GetDeviceRuntimeSummaryAsync(Guid deviceId, DateTime? from = null, DateTime? to = null)
        {
            from ??= DateTime.Today;
            to ??= DateTime.Now;

            if (from >= to)
            {
                return new DeviceRuntimeSummary();
            }

            // 取得並排序原始紀錄
            var runtimeRecords = (await _deviceRuntimeRepository.GetRecordsAsync(deviceId, from.Value, to.Value))
                .OrderBy(record => record.StartTime)
                .ToList();

            // 正規化時間軸（補 Offline）
            var normalizedRuntimeRecords = new List<DeviceRuntimeRecord>();
            var cursor = from.Value;
            foreach (var runtimeRecord in runtimeRecords)
            {
                // 前一段與此段之間有間隔 → 補 Offline
                if (runtimeRecord.StartTime > cursor)
                {
                    normalizedRuntimeRecords.Add(new DeviceRuntimeRecord(
                        deviceId,
                        DeviceRunStatus.Offline,
                        cursor,
                        runtimeRecord.StartTime));

                    cursor = runtimeRecord.StartTime;
                }

                // 插入此段
                var start = runtimeRecord.StartTime < cursor ? cursor : runtimeRecord.StartTime;
                var end = runtimeRecord.EndTime > to ? to.Value : runtimeRecord.EndTime;
                if (start < end)
                {
                    normalizedRuntimeRecords.Add(new DeviceRuntimeRecord(
                        deviceId,
                        runtimeRecord.RunStatus,
                        start,
                        end));

                    cursor = end;
                }
            }

            // 尾段補 Offline
            if (cursor < to)
            {
                normalizedRuntimeRecords.Add(new DeviceRuntimeRecord(
                    deviceId,
                    DeviceRunStatus.Offline,
                    cursor,
                    to.Value));
            }

            // 計算占比
            var totalSeconds = (to.Value - from.Value).TotalSeconds;
            var offlineSeconds = normalizedRuntimeRecords
                .Where(r => r.RunStatus == DeviceRunStatus.Offline)
                .Sum(r => r.RunDuration.TotalSeconds);
            var standbySeconds = normalizedRuntimeRecords
                .Where(r => r.RunStatus == DeviceRunStatus.Standby)
                .Sum(r => r.RunDuration.TotalSeconds);
            var runningSeconds = normalizedRuntimeRecords
                .Where(r => r.RunStatus == DeviceRunStatus.Running)
                .Sum(r => r.RunDuration.TotalSeconds);
            var alarmSeconds = normalizedRuntimeRecords
                .Where(r => r.RunStatus == DeviceRunStatus.Alarm)
                .Sum(r => r.RunDuration.TotalSeconds);

            // 回傳結果
            return new DeviceRuntimeSummary
            {
                RuntimeRecords = normalizedRuntimeRecords,
                OfflineUtilization = offlineSeconds / totalSeconds,
                StandbyUtilization = standbySeconds / totalSeconds,
                RunningUtilization = runningSeconds / totalSeconds,
                AlarmUtilization = alarmSeconds / totalSeconds
            };
        }
    }
}
