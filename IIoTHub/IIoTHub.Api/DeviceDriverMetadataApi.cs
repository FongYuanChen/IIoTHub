using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Models.DeviceSettings;
using Microsoft.AspNetCore.Mvc;

namespace IIoTHub.Api
{
    /// <summary>
    /// 設備驅動器資料相關接口
    /// </summary>
    [Route("api/deviceDriverMetadata")]
    [ApiController]
    public class DeviceDriverMetadataApi : ControllerBase
    {
        private readonly IDeviceDriverMetadataProvider _deviceDriverMetadataProvider;

        public DeviceDriverMetadataApi(IDeviceDriverMetadataProvider deviceDriverMetadataProvider)
        {
            _deviceDriverMetadataProvider = deviceDriverMetadataProvider;
        }

        /// <summary>
        /// 取得機台驅動器資料列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("machine")]
        public IEnumerable<DeviceDriverSetting> GetMachineDriverMetadata()
        {
            return _deviceDriverMetadataProvider.GetDriverMetadata(DeviceCategoryType.Machine);
        }

        /// <summary>
        /// 取得料倉驅動器資料列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("magazine")]
        public IEnumerable<DeviceDriverSetting> GetMagazineDriverMetadata()
        {
            return _deviceDriverMetadataProvider.GetDriverMetadata(DeviceCategoryType.Magazine);
        }

        /// <summary>
        /// 取得機台驅動器資料列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("robot")]
        public IEnumerable<DeviceDriverSetting> GetRobotDriverMetadata()
        {
            return _deviceDriverMetadataProvider.GetDriverMetadata(DeviceCategoryType.Robot);
        }
    }
}
