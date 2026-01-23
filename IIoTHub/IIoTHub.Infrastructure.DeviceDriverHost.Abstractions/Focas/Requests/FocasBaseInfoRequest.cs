using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Enums;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Requests
{
    /// <summary>
    /// FOCAS 資訊請求基底
    /// </summary>
    public class FocasBaseInfoRequest
    {
        public FocasBaseInfoRequest(FocasCommandType commandType)
        {
            CommandType = commandType;
        }

        /// <summary>
        /// FOCAS 指令類型
        /// </summary>
        public FocasCommandType CommandType { get; }

        /// <summary>
        /// 請求唯一識別碼
        /// </summary>
        public Guid RequestId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 控制器 IP 位址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 控制器連接埠號
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 連線逾時時間 (秒)
        /// </summary>
        public int Timeout { get; set; }
    }
}
