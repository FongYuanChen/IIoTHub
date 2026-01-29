using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Models;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Responses
{
    /// <summary>
    /// FCSB1224W000 主軸資訊請求回應
    /// </summary>
    public class FCSB1224W000SpindleInfoResponse : FCSB1224W000BaseInfoResponse
    {
        /// <summary>
        /// 主軸資訊
        /// </summary>
        public FCSB1224W000SpindleInfo SpindleInfo { get; set; }

        /// <summary>
        /// 建立一個成功的回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="spindleInfo"></param>
        /// <returns></returns>
        public static FCSB1224W000SpindleInfoResponse Success(Guid requestId, FCSB1224W000SpindleInfo spindleInfo)
        {
            return new FCSB1224W000SpindleInfoResponse
            {
                IsSuccess = true,
                RequestId = requestId,
                SpindleInfo = spindleInfo
            };
        }

        /// <summary>
        /// 建立一個例外回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static FCSB1224W000SpindleInfoResponse FromException(Guid requestId, Exception ex)
        {
            return new FCSB1224W000SpindleInfoResponse
            {
                IsSuccess = false,
                RequestId = requestId,
                Exception = ex
            };
        }
    }
}
