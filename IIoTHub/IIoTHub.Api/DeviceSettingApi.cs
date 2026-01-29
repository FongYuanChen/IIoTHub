using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Models.DeviceSettings;
using Microsoft.AspNetCore.Mvc;

namespace IIoTHub.Api
{
    /// <summary>
    /// 設備設定相關接口
    /// </summary>
    [Route("api/deviceSetting")]
    [ApiController]
    public class DeviceSettingApi : ControllerBase
    {
        private readonly IDeviceSettingService _deviceSettingService;

        public DeviceSettingApi(IDeviceSettingService deviceSettingService)
        {
            _deviceSettingService = deviceSettingService;
        }

        /// <summary>
        /// 取得所有設備設定
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<DeviceSetting>> GetAllAsync()
        {
            return await _deviceSettingService.GetAllAsync();
        }

        /// <summary>
        /// 取得指定的設備設定
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet("{deviceId}")]
        public async Task<DeviceSetting> GetByIdAsync(Guid deviceId)
        {
            return await _deviceSettingService.GetByIdAsync(deviceId);
        }

        /// <summary>
        /// 新增設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        [HttpPost]
        public async Task AddAsync(DeviceSetting deviceSetting)
        {
            await _deviceSettingService.AddAsync(deviceSetting);
        }

        /// <summary>
        /// 更新設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        [HttpPut]
        public async Task UpdateAsync(DeviceSetting deviceSetting)
        {
            await _deviceSettingService.UpdateAsync(deviceSetting);
        }

        /// <summary>
        /// 刪除指定的設備設定
        /// </summary>
        /// <param name="deviceId"></param>
        [HttpDelete]
        public async Task DeleteAsync(Guid deviceId)
        {
            await _deviceSettingService.DeleteAsync(deviceId);
        }
    }
}
