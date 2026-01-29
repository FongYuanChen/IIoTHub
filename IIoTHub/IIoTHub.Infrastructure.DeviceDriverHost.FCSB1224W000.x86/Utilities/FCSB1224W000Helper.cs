using EZNCAUTLib;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Enums;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Models;

namespace IIoTHub.Infrastructure.DeviceDriverHost.FCSB1224W000.x86.Utilities
{
    /// <summary>
    /// FCSB1224W000 操作輔助方法
    /// </summary>
    public class FCSB1224W000Helper
    {
        /// <summary>
        /// 取得狀態資訊
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        public static FCSB1224W000StateInfo GetStateInfo(DispEZNcCommunication communication)
        {
            return new FCSB1224W000StateInfo
            {
                RunStatus = GetRunStatus(communication),
                OperatingMode = GetOperatingMode(communication)
            };
        }

        /// <summary>
        /// 取得運轉狀態
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        private static FCSB1224W000RunStatus GetRunStatus(DispEZNcCommunication communication)
        {
            var alarmRawMessage = GetAlarmRawMessage(communication);
            if (!string.IsNullOrWhiteSpace(alarmRawMessage))
            {
                return FCSB1224W000RunStatus.Alarm;
            }
            else
            {
                EnsureOk(communication.Status_GetRunStatus(FCSB1224W000Parameter.EZNC_OP, out var runSignal));
                EnsureOk(communication.Status_GetRunStatus(FCSB1224W000Parameter.EZNC_STL, out var startSignal));
                EnsureOk(communication.Status_GetRunStatus(FCSB1224W000Parameter.EZNC_SPL, out var pauseSignal));
                if (runSignal == FCSB1224W000Parameter.EZ_FALSE && startSignal == FCSB1224W000Parameter.EZ_FALSE && pauseSignal == FCSB1224W000Parameter.EZ_FALSE)
                {
                    return FCSB1224W000RunStatus.Reset;
                }
                else if (runSignal == FCSB1224W000Parameter.EZ_TRUE && startSignal == FCSB1224W000Parameter.EZ_FALSE && pauseSignal == FCSB1224W000Parameter.EZ_TRUE)
                {
                    return FCSB1224W000RunStatus.Hold;
                }
                else if (runSignal == FCSB1224W000Parameter.EZ_TRUE && startSignal == FCSB1224W000Parameter.EZ_TRUE && pauseSignal == FCSB1224W000Parameter.EZ_FALSE)
                {
                    // Get M code(commanded independently).
                    EnsureOk(communication.Command_GetCommand2(FCSB1224W000Parameter.EZNC_M, 1, out var mCode));
                    if (mCode == 0 || mCode == 1) // M00(Program stop), M01(Optional stop)
                    {
                        return FCSB1224W000RunStatus.Stop;
                    }
                    else
                    {
                        return FCSB1224W000RunStatus.Start;
                    }
                }
                else
                {
                    return FCSB1224W000RunStatus.Stop;
                }
            }
        }

        /// <summary>
        /// 取得操作模式
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        private static string GetOperatingMode(DispEZNcCommunication communication)
        {
            var handleModeSignal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_HANDLE, FCSB1224W000Parameter.EZNC_PLC_BIT);
            if (handleModeSignal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return FCSB1224W000Parameter.EZNC_MODE_DISP_HANDLE;
            }
            var incrementalModeSignal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_INC, FCSB1224W000Parameter.EZNC_PLC_BIT);
            if (incrementalModeSignal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return FCSB1224W000Parameter.EZNC_MODE_DISP_INC;
            }
            var memoryModeSignal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_MEM, FCSB1224W000Parameter.EZNC_PLC_BIT);
            if (memoryModeSignal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return FCSB1224W000Parameter.EZNC_MODE_DISP_MEM;
            }
            var mdiModeSignal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_MDI, FCSB1224W000Parameter.EZNC_PLC_BIT);
            if (mdiModeSignal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return FCSB1224W000Parameter.EZNC_MODE_DISP_MDI;
            }
            var referencePositionReturnModeSignal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_REF, FCSB1224W000Parameter.EZNC_PLC_BIT);
            var rapidTraverseSignal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_RAPID, FCSB1224W000Parameter.EZNC_PLC_BIT);
            var jogModeSignal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_JOG, FCSB1224W000Parameter.EZNC_PLC_BIT);
            if (referencePositionReturnModeSignal == FCSB1224W000Parameter.EZ_TRUE && rapidTraverseSignal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return FCSB1224W000Parameter.EZNC_MODE_DISP_REF;
            }
            if (jogModeSignal == FCSB1224W000Parameter.EZ_TRUE && rapidTraverseSignal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return FCSB1224W000Parameter.EZNC_MODE_DISP_RAPID;
            }
            if (jogModeSignal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return FCSB1224W000Parameter.EZNC_MODE_DISP_JOG;
            }
            return string.Empty;
        }

        /// <summary>
        /// 取得警報資訊
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        public static FCSB1224W000AlarmInfo GetAlarmInfo(DispEZNcCommunication communication)
        {
            var alarmRawMessage = GetAlarmRawMessage(communication);
            if (string.IsNullOrWhiteSpace(alarmRawMessage))
            {
                return new FCSB1224W000AlarmInfo();
            }
            else
            {
                return new FCSB1224W000AlarmInfo
                {
                    AlarmMessages = alarmRawMessage.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(alarmMessage =>
                                                   {
                                                       var alarmParts = alarmMessage.Split('\t').ToList();
                                                       var alarmNoIndex = 2;
                                                       var alarmNo = int.TryParse(alarmParts[alarmNoIndex], out var alarmNoValue) ? alarmNoValue : int.MaxValue;
                                                       alarmParts.RemoveAt(alarmNoIndex);
                                                       alarmParts.RemoveAll(alarmPart => string.IsNullOrEmpty(alarmPart));
                                                       return new FCSB1224W000AlarmMessage
                                                       {
                                                           No = alarmNo,
                                                           Message = string.Join(' ', alarmParts)
                                                       };
                                                   })
                                                   .ToList()
                };
            }
        }

        /// <summary>
        /// 取得原始警報字串
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        private static string GetAlarmRawMessage(DispEZNcCommunication communication)
        {
            EnsureOk(communication.System_GetAlarm2(FCSB1224W000Parameter.M_ALM_MAXNUM, FCSB1224W000Parameter.M_ALM_NC_ALARM, out string alarmMessages));
            return alarmMessages;
        }

        /// <summary>
        /// 取得程式資訊
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        public static FCSB1224W000ProgramInfo GetProgramInfo(DispEZNcCommunication communication)
        {
            EnsureOk(communication.Program_GetProgramNumber2(FCSB1224W000Parameter.EZNC_MAINPRG, out var mainProgram));
            EnsureOk(communication.Program_GetProgramNumber2(FCSB1224W000Parameter.EZNC_SUBPRG, out var subProgram));
            return new FCSB1224W000ProgramInfo
            {
                MainProgramName = mainProgram,
                ExecutingProgramName = string.IsNullOrWhiteSpace(subProgram) ? mainProgram : subProgram // 目前執行程式名稱若為空,則執行程式為主程式
            };
        }

        /// <summary>
        /// 取得主軸資訊
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        public static FCSB1224W000SpindleInfo GetSpindleInfo(DispEZNcCommunication communication)
        {
            return new FCSB1224W000SpindleInfo
            {
                FeedRateInfo = new FCSB1224W000FeedRateInfo
                {
                    Actual = GetSpindleFeedrateActual(communication),
                    Percentage = GetSpindleFeedratePercentage(communication),
                    RapidPercentage = GetSpindleRapidFeedratePercentage(communication)
                },
                SpeedInfo = new FCSB1224W000SpeedInfo
                {
                    Actual = GetSpindleSpeedActual(communication),
                    Percentage = GetSpindleSpeedPercentage(communication)
                },
                LoadInfo = new FCSB1224W000LoadInfo
                {
                    Percentage = GetSpindleLoadPercentage(communication)
                }
            };
        }

        /// <summary>
        /// 取得主軸實際進給速率
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        private static int GetSpindleFeedrateActual(DispEZNcCommunication communication)
        {
            EnsureOk(communication.Command_GetFeedCommand(FCSB1224W000Parameter.EZNC_FEED_FC, out double feedrate));
            return Convert.ToInt32(feedrate);
        }

        /// <summary>
        /// 取得主軸進給倍率
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        private static int GetSpindleFeedratePercentage(DispEZNcCommunication communication)
        {
            var code1Signal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_FEED_PCT_CODE1, FCSB1224W000Parameter.EZNC_PLC_BIT);
            var code2Signal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_FEED_PCT_CODE2, FCSB1224W000Parameter.EZNC_PLC_BIT);
            var code4Signal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_FEED_PCT_CODE4, FCSB1224W000Parameter.EZNC_PLC_BIT);
            var code8Signal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_FEED_PCT_CODE8, FCSB1224W000Parameter.EZNC_PLC_BIT);
            var code16Signal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_FEED_PCT_CODE16, FCSB1224W000Parameter.EZNC_PLC_BIT);
            if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 10;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 20;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 30;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 40;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 50;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 60;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 70;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 80;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 90;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 100;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 110;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 120;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 130;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 140;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 150;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 160;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 170;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 180;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 190;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 200;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 210;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 220;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_TRUE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 230;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 240;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 250;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 260;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_TRUE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 270;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 280;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 290;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_FALSE && code8Signal == FCSB1224W000Parameter.EZ_FALSE && code16Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 300;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 取得主軸快速進給倍率
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        private static int GetSpindleRapidFeedratePercentage(DispEZNcCommunication communication)
        {
            var code1Signal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_RAPID_PCT_CODE1, FCSB1224W000Parameter.EZNC_PLC_BIT);
            var code2Signal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_RAPID_PCT_CODE2, FCSB1224W000Parameter.EZNC_PLC_BIT);
            if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 100;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 50;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 25;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// 取得主軸實際轉速
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        private static int GetSpindleSpeedActual(DispEZNcCommunication communication)
        {
            EnsureOk(communication.Monitor_GetSpindleMonitor(FCSB1224W000Parameter.EZNC_SPINDLE_MONITOR_SPEED, 1, out var spindleSpeed, out _));
            return spindleSpeed;
        }

        /// <summary>
        /// 取得主軸轉速倍率
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        private static int GetSpindleSpeedPercentage(DispEZNcCommunication communication)
        {
            var code1Signal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_SP_PCT_CODE1, FCSB1224W000Parameter.EZNC_PLC_BIT);
            var code2Signal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_SP_PCT_CODE2, FCSB1224W000Parameter.EZNC_PLC_BIT);
            var code4Signal = ReadDevice(communication, FCSB1224W000Parameter.EZNC_PLC_SP_PCT_CODE4, FCSB1224W000Parameter.EZNC_PLC_BIT);
            if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 50;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 60;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 70;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_TRUE && code4Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 80;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_TRUE)
            {
                return 90;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_FALSE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 100;
            }
            else if (code1Signal == FCSB1224W000Parameter.EZ_TRUE && code2Signal == FCSB1224W000Parameter.EZ_FALSE && code4Signal == FCSB1224W000Parameter.EZ_FALSE)
            {
                return 110;
            }
            else
            {
                return 120;
            }
        }

        /// <summary>
        /// 取得主軸負載
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        private static int GetSpindleLoadPercentage(DispEZNcCommunication communication)
        {
            EnsureOk(communication.Monitor_GetSpindleMonitor(FCSB1224W000Parameter.EZNC_SPINDLE_MONITOR_LOAD, 1, out int spindleLoad, out _));
            return spindleLoad;
        }

        /// <summary>
        /// 取得位置資訊
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        public static FCSB1224W000PositionInfo GetPositionInfo(DispEZNcCommunication communication)
        {
            return new FCSB1224W000PositionInfo
            {
                MachinePosition = GetMachinePosition(communication),
                RelativePosition = GetRelativePosition(communication),
                DistanceToGo = GetDistanceToGo(communication)
            };
        }

        /// <summary>
        /// 取得機械座標位置
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        private static Dictionary<string, double> GetMachinePosition(DispEZNcCommunication communication)
        {
            EnsureOk(communication.System_GetSystemInformation(FCSB1224W000Parameter.EZNC_SYS_INFO_AXIS, out int axisNumber));
            return Enumerable.Range(1, axisNumber)
                             .Select(axisNo =>
                             {
                                 EnsureOk(communication.Parameter_GetData3(0, FCSB1224W000Parameter.EZNC_PARM_AXIS_NAME, 1, axisNo, out var parameterData));
                                 var axisName = ((string[])parameterData)[0];
                                 EnsureOk(communication.Position_GetMachinePosition2(axisNo, out var axisPosition, FCSB1224W000Parameter.EZ_TRUE));
                                 return new KeyValuePair<string, double>(axisName, axisPosition);
                             })
                             .ToDictionary();
        }

        /// <summary>
        /// 取得相對座標位置
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        private static Dictionary<string, double> GetRelativePosition(DispEZNcCommunication communication)
        {
            EnsureOk(communication.System_GetSystemInformation(FCSB1224W000Parameter.EZNC_SYS_INFO_AXIS, out int axisNumber));
            return Enumerable.Range(1, axisNumber)
                             .Select(axisNo =>
                             {
                                 EnsureOk(communication.Parameter_GetData3(0, FCSB1224W000Parameter.EZNC_PARM_AXIS_NAME, 1, axisNo, out var parameterData));
                                 var axisName = ((string[])parameterData)[0];
                                 EnsureOk(communication.Position_GetCurrentPosition(axisNo, out var axisPosition));
                                 return new KeyValuePair<string, double>(axisName, axisPosition);
                             })
                             .ToDictionary();
        }

        /// <summary>
        /// 取得剩餘移動距離
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        private static Dictionary<string, double> GetDistanceToGo(DispEZNcCommunication communication)
        {
            EnsureOk(communication.System_GetSystemInformation(FCSB1224W000Parameter.EZNC_SYS_INFO_AXIS, out int axisNumber));
            return Enumerable.Range(1, axisNumber)
                             .Select(axisNo =>
                             {
                                 EnsureOk(communication.Parameter_GetData3(0, FCSB1224W000Parameter.EZNC_PARM_AXIS_NAME, 1, axisNo, out var parameterData));
                                 var axisName = ((string[])parameterData)[0];
                                 EnsureOk(communication.Position_GetDistance2(axisNo, out var axisPosition, FCSB1224W000Parameter.EZ_FALSE));
                                 return new KeyValuePair<string, double>(axisName, axisPosition);
                             })
                             .ToDictionary();
        }

        /// <summary>
        /// 讀取軟元件
        /// </summary>
        /// <param name="communication"></param>
        /// <param name="device"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private static int ReadDevice(DispEZNcCommunication communication, string device, int dataType)
        {
            EnsureOk(communication.Device_SetDevice(new object[] { device },
                                                    new object[] { dataType },
                                                    new object[]
                                                    {
                                                        dataType switch
                                                        {
                                                            FCSB1224W000Parameter.EZNC_PLC_BIT => FCSB1224W000Parameter.EZNC_PLC_BIT_FLG,
                                                            FCSB1224W000Parameter.EZNC_PLC_BYTE => FCSB1224W000Parameter.EZNC_PLC_BYTE_FLG,
                                                            FCSB1224W000Parameter.EZNC_PLC_WORD => FCSB1224W000Parameter.EZNC_PLC_WORD_FLG,
                                                            FCSB1224W000Parameter.EZNC_PLC_DWORD => FCSB1224W000Parameter.EZNC_PLC_DWORD_FLG,
                                                            _ => default
                                                        }
                                                    }));
            EnsureOk(communication.Device_Read(out var values));
            return ((int[])values)[0];
        }

        /// <summary>
        /// 驗證 FCSB1224W000 呼叫結果碼
        /// </summary>
        /// <param name="resultCode"></param>
        /// <exception cref="FCSB1224W000Exception"></exception>
        public static void EnsureOk(int resultCode)
        {
            if (resultCode != FCSB1224W000Parameter.S_OK)
                throw new FCSB1224W000Exception(resultCode);
        }
    }
}
