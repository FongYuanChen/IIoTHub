using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Models;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Responses
{
    /// <summary>
    /// FCSB1224W000 機台狀態資訊請求回應
    /// </summary>
    public class FCSB1224W000StateInfoResponse : FCSB1224W000BaseInfoResponse
    {
        /// <summary>
        /// 運行狀態資訊
        /// </summary>
        public FCSB1224W000StateInfo StateInfo { get; set; }

        /// <summary>
        /// 建立一個成功的回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="stateInfo"></param>
        /// <returns></returns>
        public static FCSB1224W000StateInfoResponse Success(Guid requestId, FCSB1224W000StateInfo stateInfo)
        {
            return new FCSB1224W000StateInfoResponse
            {
                IsSuccess = true,
                RequestId = requestId,
                StateInfo = stateInfo
            };
        }

        /// <summary>
        /// 建立一個例外回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static FCSB1224W000StateInfoResponse FromException(Guid requestId, Exception ex)
        {
            return new FCSB1224W000StateInfoResponse
            {
                IsSuccess = false,
                RequestId = requestId,
                Exception = ex
            };
        }
    }
}
