using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Enums;

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
    }
}
