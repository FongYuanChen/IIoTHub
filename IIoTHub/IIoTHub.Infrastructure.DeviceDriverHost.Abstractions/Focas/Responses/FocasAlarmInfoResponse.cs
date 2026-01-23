using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Responses
{
    /// <summary>
    /// FOCAS 警報資訊請求回應
    /// </summary>
    public class FocasAlarmInfoResponse : FocasBaseInfoResponse
    {
        /// <summary>
        /// 警報資訊
        /// </summary>
        public FocasAlarmInfo AlarmInfo { get; set; }

        /// <summary>
        /// 建立一個成功的回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="alarmInfo"></param>
        /// <returns></returns>
        public static FocasAlarmInfoResponse Success(Guid requestId, FocasAlarmInfo alarmInfo)
        {
            return new FocasAlarmInfoResponse
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
        public static FocasAlarmInfoResponse FromException(Guid requestId, Exception ex)
        {
            return new FocasAlarmInfoResponse
            {
                IsSuccess = false,
                RequestId = requestId,
                Exception = ex
            };
        }
    }
}
