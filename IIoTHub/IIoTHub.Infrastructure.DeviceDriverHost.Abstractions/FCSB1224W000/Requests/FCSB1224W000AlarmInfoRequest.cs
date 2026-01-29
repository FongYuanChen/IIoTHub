using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Enums;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Requests
{
    /// <summary>
    /// FCSB1224W000 警報資訊請求
    /// </summary>
    public class FCSB1224W000AlarmInfoRequest : FCSB1224W000BaseInfoRequest
    {
        public FCSB1224W000AlarmInfoRequest() : base(FCSB1224W000CommandType.GetAlarmInfo)
        {
        }
    }
}
