namespace IIoTHub.Infrastructure.DeviceDriverHost.FCSB1224W000.x86.Utilities
{
    public static class FCSB1224W000Parameter
    {
        #region True/False

        public const int EZ_TRUE = 0x1;
        public const int EZ_FALSE = 0x0;

        #endregion

        #region ResultCode

        public const int S_OK = 0x0;

        #endregion

        #region Open3

        public const int EZNC_SYS_MELDAS700L = 5;
        public const int EZNC_SYS_MELDAS700M = 6;
        public const int EZNC_SYS_MELDASC70 = 7;
        public const int EZNC_SYS_MELDAS800L = 8;
        public const int EZNC_SYS_MELDAS800M = 9;
        public const int EZNC_SYS_MELDASC80 = 10;
        public const int EZNC_SYS_MULTI = 0x00010000;

        #endregion

        #region Close2

        public const int EZNC_RESET_NONE = 0;
        public const int EZNC_RESET_SIMPLE = 1;
        public const int EZNC_RESET_ALL = 2;

        #endregion

        #region System_GetSystemInformation

        public const int EZNC_SYS_INFO_AXIS = 1;

        #endregion

        #region System_GetAlarm2

        public const int M_ALM_ALL_ALARM = 0x000;
        public const int M_ALM_NC_ALARM = 0x100;
        public const int M_ALM_STOP_CODE = 0x200;
        public const int M_ALM_PLC_ALARM = 0x300;
        public const int M_ALM_OPE_MSG = 0x400;
        public const int M_ALM_MAXNUM = 10;

        #endregion

        #region Command_GetFeedCommand

        public const int EZNC_FEED_FA = 0;
        public const int EZNC_FEED_FM = 1;
        public const int EZNC_FEED_FS = 2;
        public const int EZNC_FEED_FC = 3;
        public const int EZNC_FEED_FE = 4;
        public const int EZNC_FEED_TCP = 5;

        #endregion

        #region Command_GetCommand2

        public const int EZNC_M = 0;
        public const int EZNC_S = 1;
        public const int EZNC_T = 2;
        public const int EZNC_B = 3;

        #endregion

        #region Program_GetProgramNumber2

        public const int EZNC_MAINPRG = 0;
        public const int EZNC_SUBPRG = 1;

        #endregion

        #region Monitor_GetSpindleMonitor

        public const int EZNC_SPINDLE_MONITOR_SPEED = 2;
        public const int EZNC_SPINDLE_MONITOR_LOAD = 3;

        #endregion

        #region Status_GetRunStatus

        public const int EZNC_OP = 1;
        public const int EZNC_STL = 2;
        public const int EZNC_SPL = 3;

        #endregion

        #region Device_SetDevice

        public const int EZNC_PLC_1SHOT = 0x10;
        public const int EZNC_PLC_MODAL = 0x20;
        public const int EZNC_PLC_BIT_FLG = 0x01;
        public const int EZNC_PLC_BYTE_FLG = 0x02;
        public const int EZNC_PLC_WORD_FLG = 0x04;
        public const int EZNC_PLC_DWORD_FLG = 0x08;
        public const int EZNC_PLC_BIT = (0x01 | 0x10);
        public const int EZNC_PLC_BYTE = (0x02 | 0x10);
        public const int EZNC_PLC_WORD = (0x04 | 0x10);
        public const int EZNC_PLC_DWORD = (0x08 | 0x10);

        #endregion

        #region Device_SetDevice / Device_Read Item

        public const string EZNC_PLC_JOG = "YC00";
        public const string EZNC_PLC_HANDLE = "YC01";
        public const string EZNC_PLC_INC = "YC02";
        public const string EZNC_PLC_REF = "YC04";
        public const string EZNC_PLC_MEM = "YC08";
        public const string EZNC_PLC_MDI = "YC0B";
        public const string EZNC_PLC_RAPID = "YC26";
        public const string EZNC_PLC_FEED_PCT_CODE1 = "YC60";
        public const string EZNC_PLC_FEED_PCT_CODE2 = "YC61";
        public const string EZNC_PLC_FEED_PCT_CODE4 = "YC62";
        public const string EZNC_PLC_FEED_PCT_CODE8 = "YC63";
        public const string EZNC_PLC_FEED_PCT_CODE16 = "YC64";
        public const string EZNC_PLC_RAPID_PCT_CODE1 = "YC68";
        public const string EZNC_PLC_RAPID_PCT_CODE2 = "YC69";
        public const string EZNC_PLC_SP_PCT_CODE1 = "Y1888";
        public const string EZNC_PLC_SP_PCT_CODE2 = "Y1889";
        public const string EZNC_PLC_SP_PCT_CODE4 = "Y188A";

        #endregion

        #region Parameter_GetData3 Item

        public const int EZNC_PARM_AXIS_NAME = 1013;

        #endregion

        #region Mode

        public const string EZNC_MODE_DISP_JOG = "JOG";
        public const string EZNC_MODE_DISP_HANDLE = "HANDLE";
        public const string EZNC_MODE_DISP_INC = "INC";
        public const string EZNC_MODE_DISP_REF = "REF";
        public const string EZNC_MODE_DISP_MEM = "MEM";
        public const string EZNC_MODE_DISP_MDI = "MDI";
        public const string EZNC_MODE_DISP_RAPID = "RAPID";

        #endregion
    }
}
