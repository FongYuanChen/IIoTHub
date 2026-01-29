namespace IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Models
{
    /// <summary>
    /// FCSB1224W000 程式資訊
    /// </summary>
    public class FCSB1224W000ProgramInfo
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
