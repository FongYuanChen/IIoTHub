using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Models;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Responses
{
    /// <summary>
    /// FCSB1224W000 警報資訊請求回應
    /// </summary>
    public class FCSB1224W000AlarmInfoResponse : FCSB1224W000BaseInfoResponse
    {
        /// <summary>
        /// 警報資訊
        /// </summary>
        public FCSB1224W000AlarmInfo AlarmInfo { get; set; }

        /// <summary>
        /// 建立一個成功的回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="alarmInfo"></param>
        /// <returns></returns>
        public static FCSB1224W000AlarmInfoResponse Success(Guid requestId, FCSB1224W000AlarmInfo alarmInfo)
        {
            return new FCSB1224W000AlarmInfoResponse
            {
                IsSuccess = true,
                RequestId = requestId,
                AlarmInfo = alarmInfo
            };
        }

        /// <summary>
        /// 建立一個例外回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static FCSB1224W000AlarmInfoResponse FromException(Guid requestId, Exception ex)
        {
            return new FCSB1224W000AlarmInfoResponse
            {
                IsSuccess = false,
                RequestId = requestId,
                Exception = ex
            };
        }
    }
}
