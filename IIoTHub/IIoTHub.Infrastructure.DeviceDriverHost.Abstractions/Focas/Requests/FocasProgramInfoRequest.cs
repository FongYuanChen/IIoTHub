using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Enums;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Requests
{
    /// <summary>
    /// FOCAS 程式資訊請求
    /// </summary>
    public class FocasProgramInfoRequest : FocasBaseInfoRequest
    {
        public FocasProgramInfoRequest() : base(FocasCommandType.GetProgramInfo)
        {
        }
    }
}
