using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Models;

namespace IIoTHub.Application.Services
{
    public class DeviceRuntimeStatisticsService : IDeviceRuntimeStatisticsService
    {
        private readonly IDeviceRuntimeRepository _repository;

        public DeviceRuntimeStatisticsService(IDeviceRuntimeRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 處理設備快照事件，依據狀態變化更新設備運轉紀錄
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public async Task OnSnapshotAsync(DeviceSnapshot snapshot)
        {
            var last = await _repository.GetLatestRecordAsync(snapshot.DeviceId);
            if (last == null || last.RunStatus != snapshot.RunStatus)
            {
                // 建立新紀錄
                var record = new DeviceRuntimeRecord(snapshot.DeviceId, snapshot.RunStatus, snapshot.Timestamp, snapshot.Timestamp);
                await _repository.AddAsync(record);
            }
            else
            {
                // 狀態沒變 → 延長上一筆
                var record = new DeviceRuntimeRecord(snapshot.DeviceId, snapshot.RunStatus, last.StartTime, snapshot.Timestamp);
                await _repository.UpdateAsync(record);
            }
        }

        /// <summary>
        /// 取得指定設備在時間區間內的稼動率 (稼動率定義為 Running 狀態時間佔總時間的比例)
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<double> GetUtilizationAsync(Guid deviceId, DateTime? from = null, DateTime? to = null)
        {
            to ??= DateTime.Now;

            // 先抓出範圍內所有資料
            var records = await _repository.GetRecordsAsync(deviceId, from ?? DateTime.MinValue, to.Value);
            if (!records.Any())
                return 0;

            // 如果 from 沒傳，從最早一筆開始
            var fromTime = from ?? records.Min(r => r.StartTime);

            double totalSeconds = (to.Value - fromTime).TotalSeconds;
            if (totalSeconds <= 0)
                return 0;

            // 計算稼動秒數（只有 Running 算稼動）
            double runningSeconds = 0;
            foreach (var record in records)
            {
                if (record.RunStatus == DeviceRunStatus.Running)
                    runningSeconds += (record.EndTime - record.StartTime).TotalSeconds;
            }

            return runningSeconds / totalSeconds;
        }
    }
}
