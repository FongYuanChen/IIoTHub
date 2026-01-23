using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Enums;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Requests
{
    /// <summary>
    /// FOCAS 位置資訊請求
    /// </summary>
    public class FocasPositionInfoRequest : FocasBaseInfoRequest
    {
        public FocasPositionInfoRequest() : base(FocasCommandType.GetPositionInfo)
        {
        }
    }
}
