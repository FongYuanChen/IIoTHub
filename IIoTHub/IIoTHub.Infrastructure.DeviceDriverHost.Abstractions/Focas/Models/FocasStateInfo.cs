using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Enums;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models
{
    /// <summary>
    /// FOCAS 狀態資訊
    /// </summary>
    public class FocasStateInfo
    {
        /// <summary>
        /// 運轉狀態
        /// </summary>
        public FocasRunStatus RunStatus { get; set; }

        /// <summary>
        /// 操作模式
        /// </summary>
        public string OperatingMode { get; set; }
    }
}
