using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Enums;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Requests
{
    /// <summary>
    /// FCSB1224W000 主軸資訊請求
    /// </summary>
    public class FCSB1224W000SpindleInfoRequest : FCSB1224W000BaseInfoRequest
    {
        public FCSB1224W000SpindleInfoRequest() : base(FCSB1224W000CommandType.GetSpindleInfo)
        {
        }
    }
}
