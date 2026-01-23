using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Responses
{
    /// <summary>
    /// FOCAS 機台位置資訊請求回應
    /// </summary>
    public class FocasPositionInfoResponse : FocasBaseInfoResponse
    {
        /// <summary>
        /// 位置資訊
        /// </summary>
        public FocasPositionInfo PositionInfo { get; set; }

        /// <summary>
        /// 建立一個成功的回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="positionInfo"></param>
        /// <returns></returns>
        public static FocasPositionInfoResponse Success(Guid requestId, FocasPositionInfo positionInfo)
        {
            return new FocasPositionInfoResponse
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
        public static FocasPositionInfoResponse FromException(Guid requestId, Exception ex)
        {
            return new FocasPositionInfoResponse
            {
                IsSuccess = false,
                RequestId = requestId,
                Exception = ex
            };
        }
    }
}
