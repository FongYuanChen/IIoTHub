using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Models;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Responses
{
    /// <summary>
    /// FCSB1224W000 程式資訊請求回應
    /// </summary>
    public class FCSB1224W000ProgramInfoResponse : FCSB1224W000BaseInfoResponse
    {
        /// <summary>
        /// 程式資訊
        /// </summary>
        public FCSB1224W000ProgramInfo ProgramInfo { get; set; }

        /// <summary>
        /// 建立一個成功的回應
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="programInfo"></param>
        /// <returns></returns>
        public static FCSB1224W000ProgramInfoResponse Success(Guid requestId, FCSB1224W000ProgramInfo programInfo)
        {
            return new FCSB1224W000ProgramInfoResponse
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
        public static FCSB1224W000ProgramInfoResponse FromException(Guid requestId, Exception ex)
        {
            return new FCSB1224W000ProgramInfoResponse
            {
                IsSuccess = false,
                RequestId = requestId,
                Exception = ex
            };
        }
    }
}
