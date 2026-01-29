using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Enums;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Requests
{
    /// <summary>
    /// FCSB1224W000 狀態資訊請求
    /// </summary>
    public class FCSB1224W000StateInfoRequest : FCSB1224W000BaseInfoRequest
    {
        public FCSB1224W000StateInfoRequest() : base(FCSB1224W000CommandType.GetStateInfo)
        {
        }
    }
}
