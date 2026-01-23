namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Responses
{
    /// <summary>
    /// FOCAS 資訊回應基底
    /// </summary>
    public class FocasBaseInfoResponse
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
