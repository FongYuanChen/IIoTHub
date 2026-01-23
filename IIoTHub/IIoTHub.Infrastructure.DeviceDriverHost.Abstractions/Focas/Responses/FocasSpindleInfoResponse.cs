using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Responses
{
    /// <summary>
    /// FOCAS 主軸資訊請求回應
    /// </summary>
    public class FocasSpindleInfoResponse : FocasBaseInfoResponse
    {
        /// <summary>
        /// 主軸資訊
        /// </summary>
        public FocasSpindleInfo SpindleInfo { get; set; }

        /// <summary>
        /// 建立一個成功的回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="spindleInfo"></param>
        /// <returns></returns>
        public static FocasSpindleInfoResponse Success(Guid requestId, FocasSpindleInfo spindleInfo)
        {
            return new FocasSpindleInfoResponse
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
        public static FocasSpindleInfoResponse FromException(Guid requestId, Exception ex)
        {
            return new FocasSpindleInfoResponse
            {
                IsSuccess = false,
                RequestId = requestId,
                Exception = ex
            };
        }
    }
}
