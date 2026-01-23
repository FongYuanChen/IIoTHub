using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Models.DeviceSettings;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace IIoTHub.Infrastructure.Repositories
{
    public class JsonDeviceSettingRepository : IDeviceSettingRepository
    {
        private readonly string _filePath = "deviceSettings.json";
        private static readonly SemaphoreSlim _fileLock = new(1, 1);
        private ImmutableList<DeviceSetting> _cache;

        /// <summary>
        /// 新增一筆設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        /// <returns></returns>
        public async Task AddAsync(DeviceSetting deviceSetting)
        {
            await _fileLock.WaitAsync();
            try
            {
                var deviceSettings = await LoadAllAndUpdateCacheInternalAsync();
                deviceSettings.Add(deviceSetting);
                await SaveAllAndUpdateCacheInternalAsync(deviceSettings);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 更新指定的設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        /// <returns></returns>
        public async Task UpdateAsync(DeviceSetting deviceSetting)
        {
            await _fileLock.WaitAsync();
            try
            {
                var deviceSettings = await LoadAllAndUpdateCacheInternalAsync();

                var updateIndex = deviceSettings.FindIndex(e => e.Id == deviceSetting.Id);
                if (updateIndex >= 0)
                {
                    deviceSettings[updateIndex] = deviceSetting;
                }
                else
                {
                    deviceSettings.Add(deviceSetting);
                }

                await SaveAllAndUpdateCacheInternalAsync(deviceSettings);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 刪除指定的設備設定
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid deviceId)
        {
            await _fileLock.WaitAsync();
            try
            {
                var deviceSettings = await LoadAllAndUpdateCacheInternalAsync();
                deviceSettings.RemoveAll(e => e.Id == deviceId);
                await SaveAllAndUpdateCacheInternalAsync(deviceSettings);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 取得所有的設備設定
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DeviceSetting>> GetAllAsync()
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
        /// 取得指定的設備設定
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<DeviceSetting> GetByIdAsync(Guid deviceId)
        {
            var deviceSettings = await GetAllAsync();
            return deviceSettings.FirstOrDefault(d => d.Id == deviceId);
        }

        /// <summary>
        /// 從檔案載入所有設備設定，並更新快取
        /// </summary>
        private async Task<List<DeviceSetting>> LoadAllAndUpdateCacheInternalAsync()
        {
            if (!File.Exists(_filePath))
            {
                UpdateCache([]);
                return [];
            }

            var json = await File.ReadAllTextAsync(_filePath);
            var deviceSettings = JsonConvert.DeserializeObject<List<DeviceSetting>>(json) ?? [];

            UpdateCache(deviceSettings);

            return deviceSettings;
        }

        /// <summary>
        /// 將所有設備設定寫回檔案，並更新快取
        /// </summary>
        private async Task SaveAllAndUpdateCacheInternalAsync(List<DeviceSetting> deviceSettings)
        {
            var json = JsonConvert.SerializeObject(deviceSettings, Formatting.Indented);
            await File.WriteAllTextAsync(_filePath, json);

            UpdateCache(deviceSettings);
        }

        /// <summary>
        /// 更新快取
        /// </summary>
        private void UpdateCache(List<DeviceSetting> deviceSettings)
        {
            _cache = deviceSettings.ToImmutableList();
        }
    }
}
