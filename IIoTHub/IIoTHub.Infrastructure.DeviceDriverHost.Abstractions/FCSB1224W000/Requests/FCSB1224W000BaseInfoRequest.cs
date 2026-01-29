using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Enums;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Requests
{
    /// <summary>
    /// FCSB1224W000 資訊請求基底
    /// </summary>
    public class FCSB1224W000BaseInfoRequest
    {
        public FCSB1224W000BaseInfoRequest(FCSB1224W000CommandType commandType)
        {
            CommandType = commandType;
        }

        /// <summary>
        /// FCSB1224W000 指令類型
        /// </summary>
        public FCSB1224W000CommandType CommandType { get; }

        /// <summary>
        /// 請求唯一識別碼
        /// </summary>
        public Guid RequestId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 系統類型
        /// </summary>
        public FCSB1224W000SystemType SystemType { get; set; }

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
