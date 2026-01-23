using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Responses
{
    /// <summary>
    /// FOCAS 機台狀態資訊請求回應
    /// </summary>
    public class FocasStateInfoResponse : FocasBaseInfoResponse
    {
        /// <summary>
        /// 運行狀態資訊
        /// </summary>
        public FocasStateInfo StateInfo { get; set; }

        /// <summary>
        /// 建立一個成功的回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="stateInfo"></param>
        /// <returns></returns>
        public static FocasStateInfoResponse Success(Guid requestId, FocasStateInfo stateInfo)
        {
            return new FocasStateInfoResponse
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
        public static FocasStateInfoResponse FromException(Guid requestId, Exception ex)
        {
            return new FocasStateInfoResponse
            {
                IsSuccess = false,
                RequestId = requestId,
                Exception = ex
            };
        }
    }
}
