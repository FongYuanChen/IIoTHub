namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Responses
{
    /// <summary>
    /// FCSB1224W000 資訊回應基底
    /// </summary>
    public class FCSB1224W000BaseInfoResponse
    {
        /// <summary>
        /// 對應請求的 RequestId，用於追蹤請求與回應
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// 回應是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 發生的例外資訊
        /// </summary>
        public Exception Exception { get; set; }
    }
}
