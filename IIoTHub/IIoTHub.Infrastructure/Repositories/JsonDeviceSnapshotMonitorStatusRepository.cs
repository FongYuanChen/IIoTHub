using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Models.DeviceMonitor;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace IIoTHub.Infrastructure.Repositories
{
    public class JsonDeviceSnapshotMonitorStatusRepository : IDeviceSnapshotMonitorStatusRepository
    {
        private readonly string _filePath = "deviceSnapshotMonitorStatuses.json";
        private static readonly SemaphoreSlim _fileLock = new(1, 1);
        private ImmutableList<DeviceMonitorStatus> _cache;

        /// <summary>
        /// 新增一筆設備監控狀態
        /// </summary>
        /// <param name="deviceMonitorStatus"></param>
        /// <returns></returns>
        public async Task AddAsync(DeviceMonitorStatus deviceMonitorStatus)
        {
            await _fileLock.WaitAsync();
            try
            {
                var deviceMonitorStatuses = await LoadAllAndUpdateCacheInternalAsync();
                deviceMonitorStatuses.Add(deviceMonitorStatus);
                await SaveAllAndUpdateCacheInternalAsync(deviceMonitorStatuses);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 更新指定的設備監控狀態
        /// </summary>
        /// <param name="deviceMonitorStatus"></param>
        /// <returns></returns>
        public async Task UpdateAsync(DeviceMonitorStatus deviceMonitorStatus)
        {
            await _fileLock.WaitAsync();
            try
            {
                var deviceMonitorStatuses = await LoadAllAndUpdateCacheInternalAsync();

                var updateIndex = deviceMonitorStatuses.FindIndex(e => e.Id == deviceMonitorStatus.Id);
                if (updateIndex >= 0)
                {
                    deviceMonitorStatuses[updateIndex] = deviceMonitorStatus;
                }
                else
                {
                    deviceMonitorStatuses.Add(deviceMonitorStatus);
                }

                await SaveAllAndUpdateCacheInternalAsync(deviceMonitorStatuses);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 刪除指定的設備監控狀態
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid deviceId)
        {
            await _fileLock.WaitAsync();
            try
            {
                var deviceMonitorStatuses = await LoadAllAndUpdateCacheInternalAsync();
                deviceMonitorStatuses.RemoveAll(e => e.Id == deviceId);
                await SaveAllAndUpdateCacheInternalAsync(deviceMonitorStatuses);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 取得所有的設備監控狀態
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DeviceMonitorStatus>> GetAllAsync()
        {
            if (_cache != null)
                return _cache.AsEnumerable();

            await _fileLock.WaitAsync();
            try
            {
                if (_cache == null) // double check
                    await LoadAllAndUpdateCacheInternalAsync();

                return _cache;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 取得指定的設備監控狀態
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<DeviceMonitorStatus> GetByIdAsync(Guid deviceId)
        {
            var deviceMonitorStatuses = await GetAllAsync();
            return deviceMonitorStatuses.FirstOrDefault(d => d.Id == deviceId);
        }

        /// <summary>
        /// 從檔案載入所有設備監控狀態，並更新快取
        /// </summary>
        private async Task<List<DeviceMonitorStatus>> LoadAllAndUpdateCacheInternalAsync()
        {
            if (!File.Exists(_filePath))
            {
                UpdateCache([]);
                return [];
            }

            var json = await File.ReadAllTextAsync(_filePath);
            var deviceMonitorStatuses = JsonConvert.DeserializeObject<List<DeviceMonitorStatus>>(json) ?? [];

            UpdateCache(deviceMonitorStatuses);

            return deviceMonitorStatuses;
        }

        /// <summary>
        /// 將所有設備監控狀態寫回檔案，並更新快取
        /// </summary>
        private async Task SaveAllAndUpdateCacheInternalAsync(List<DeviceMonitorStatus> deviceMonitorStatuses)
        {
            var json = JsonConvert.SerializeObject(deviceMonitorStatuses, Formatting.Indented);
            await File.WriteAllTextAsync(_filePath, json);

            UpdateCache(deviceMonitorStatuses);
        }

        /// <summary>
        /// 更新快取
        /// </summary>
        private void UpdateCache(List<DeviceMonitorStatus> deviceMonitorStatuses)
        {
            _cache = deviceMonitorStatuses.ToImmutableList();
        }
    }
}
