using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Enums;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Requests
{
    /// <summary>
    /// FOCAS 狀態資訊請求
    /// </summary>
    public class FocasStateInfoRequest : FocasBaseInfoRequest
    {
        public FocasStateInfoRequest() : base(FocasCommandType.GetStateInfo)
        {
        }
    }
}
