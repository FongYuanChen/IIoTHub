using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Enums;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Requests
{
    /// <summary>
    /// FOCAS 主軸資訊請求
    /// </summary>
    public class FocasSpindleInfoRequest : FocasBaseInfoRequest
    {
        public FocasSpindleInfoRequest() : base(FocasCommandType.GetSpindleInfo)
        {
        }

        /// <summary>
        /// 主軸相關 PMC 讀取設定
        /// </summary>
        public FocasSpindlePMCReadSetting SpindlePMCReadSetting { get; set; }
    }
}
