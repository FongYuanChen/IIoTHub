namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models
{
    /// <summary>
    /// FOCAS 程式資訊
    /// </summary>
    public class FocasProgramInfo
    {
        /// <summary>
        /// 主程式名稱
        /// </summary>
        public string MainProgramName { get; set; }

        /// <summary>
        /// 目前正在執行的程式名稱
        /// </summary>
        public string ExecutingProgramName { get; set; }
    }
}
