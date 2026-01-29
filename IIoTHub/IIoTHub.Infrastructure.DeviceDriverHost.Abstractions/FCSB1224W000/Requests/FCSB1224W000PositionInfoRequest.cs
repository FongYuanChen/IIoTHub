using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Enums;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Requests
{
    /// <summary>
    /// FCSB1224W000 位置資訊請求
    /// </summary>
    public class FCSB1224W000PositionInfoRequest : FCSB1224W000BaseInfoRequest
    {
        public FCSB1224W000PositionInfoRequest() : base(FCSB1224W000CommandType.GetPositionInfo)
        {
        }
    }
}
