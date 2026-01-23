using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Responses
{
    /// <summary>
    /// FOCAS 程式資訊請求回應
    /// </summary>
    public class FocasProgramInfoResponse : FocasBaseInfoResponse
    {
        /// <summary>
        /// 程式資訊
        /// </summary>
        public FocasProgramInfo ProgramInfo { get; set; }

        /// <summary>
        /// 建立一個成功的回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="programInfo"></param>
        /// <returns></returns>
        public static FocasProgramInfoResponse Success(Guid requestId, FocasProgramInfo programInfo)
        {
            return new FocasProgramInfoResponse
            {
                IsSuccess = true,
                RequestId = requestId,
                ProgramInfo = programInfo
            };
        }

        /// <summary>
        /// 建立一個例外回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static FocasProgramInfoResponse FromException(Guid requestId, Exception ex)
        {
            return new FocasProgramInfoResponse
            {
                IsSuccess = false,
                RequestId = requestId,
                Exception = ex
            };
        }
    }
}
