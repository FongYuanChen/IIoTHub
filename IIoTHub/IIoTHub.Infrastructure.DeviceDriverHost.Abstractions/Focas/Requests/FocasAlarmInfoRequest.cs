using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Enums;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Requests
{
    /// <summary>
    /// FOCAS 警報資訊請求
    /// </summary>
    public class FocasAlarmInfoRequest : FocasBaseInfoRequest
    {
        public FocasAlarmInfoRequest() : base(FocasCommandType.GetAlarmInfo)
        {
        }
    }
}
