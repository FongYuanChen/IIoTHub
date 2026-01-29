using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Models;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Responses
{
    /// <summary>
    /// FCSB1224W000 機台位置資訊請求回應
    /// </summary>
    public class FCSB1224W000PositionInfoResponse : FCSB1224W000BaseInfoResponse
    {
        /// <summary>
        /// 位置資訊
        /// </summary>
        public FCSB1224W000PositionInfo PositionInfo { get; set; }

        /// <summary>
        /// 建立一個成功的回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="positionInfo"></param>
        /// <returns></returns>
        public static FCSB1224W000PositionInfoResponse Success(Guid requestId, FCSB1224W000PositionInfo positionInfo)
        {
            return new FCSB1224W000PositionInfoResponse
            {
                IsSuccess = true,
                RequestId = requestId,
                PositionInfo = positionInfo
            };
        }

        /// <summary>
        /// 建立一個例外回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static FCSB1224W000PositionInfoResponse FromException(Guid requestId, Exception ex)
        {
            return new FCSB1224W000PositionInfoResponse
            {
                IsSuccess = false,
                RequestId = requestId,
                Exception = ex
            };
        }
    }
}
