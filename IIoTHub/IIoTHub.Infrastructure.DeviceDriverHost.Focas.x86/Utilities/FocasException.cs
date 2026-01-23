namespace IIoTHub.Infrastructure.DeviceDriverHost.Focas.x86.Utilities
{
    /// <summary>
    /// FOCAS 例外
    /// </summary>
    public class FocasException : Exception
    {
        public FocasException(short result) : base(GetResultInfo(result))
        {
        }

        /// <summary>
        /// 將 FOCAS 返回的結果碼轉換為可讀錯誤訊息
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static string GetResultInfo(short result)
        {
            return result switch
            {
                Focas1.EW_PROTOCOL => "Protocol error (Ethernet version only)",
                Focas1.EW_SOCKET => "Windows socket error",
                Focas1.EW_NODLL => "DLL not exist error",
                Focas1.EW_BUS => "Bus error (HSSB version only)",
                Focas1.EW_SYSTEM2 => "System error (2) (HSSB version only)",
                Focas1.EW_HSSB => "Communication error of HSSB (HSSB version only)",
                Focas1.EW_HANDLE => "Handle number error",
                Focas1.EW_VERSION => "Version mismatch between the CNC/PMC and library",
                Focas1.EW_UNEXP => "Abnormal library state",
                Focas1.EW_SYSTEM => "System error (HSSB version only)",
                Focas1.EW_PARITY => "Shared RAM parity error (HSSB version only)",
                Focas1.EW_MMCSYS => "FANUC drivers installation error (HSSB version only)",
                Focas1.EW_RESET => "Reset or stop request occured error",
                Focas1.EW_BUSY => "Busy",
                Focas1.EW_OK => "No error",
                Focas1.EW_FUNC /*and Focas1.EW_NOPMC*/ => "Function is not executed/available, or pmc not exist",
                Focas1.EW_LENGTH => "Data block length error",
                Focas1.EW_NUMBER /*and Focas1.EW_RANGE*/ => "Data number error, or address range error",
                Focas1.EW_ATTRIB /*and Focas1.EW_TYPE*/ => "Data attribute error, or data type error",
                Focas1.EW_DATA => "Data error",
                Focas1.EW_NOOPT => "No option error",
                Focas1.EW_PROT => "Write protection error",
                Focas1.EW_OVRFLOW => "Memory overflow error",
                Focas1.EW_PARAM => "CNC parameter not correct error",
                Focas1.EW_BUFFER => "Buffer empty/full error",
                Focas1.EW_PATH => "Path number error",
                Focas1.EW_MODE => "CNC mode error",
                Focas1.EW_REJECT => "CNC execution rejection error",
                Focas1.EW_DTSRVR => "Data server error",
                Focas1.EW_ALARM => "Alarm has been occurred",
                Focas1.EW_STOP => "CNC is not running",
                Focas1.EW_PASSWD => "Protection data error",
                Focas1.DNC_CANCEL => "Canceled (DNC operation)",
                Focas1.DNC_OPENERR => "File open error (DNC operation)",
                Focas1.DNC_NOFILE => "File not found (DNC operation)",
                Focas1.DNC_READERR => "Read error (DNC operation)",
                _ => $"Undefined result code, {result}"
            };
        }
    }
}
