using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Enums;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models;
using System.Text;

namespace IIoTHub.Infrastructure.DeviceDriverHost.Focas.x86.Utilities
{
    /// <summary>
    /// FOCAS 操作輔助方法
    /// </summary>
    public static class FocasHelper
    {
        /// <summary>
        /// 取得狀態資訊
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static FocasStateInfo GetStateInfo(ushort handle)
        {
            if (GetMachineType(handle) == Focas1.ODBSYS_CNC_TYPE.S15)
            {
                return GetStateInfoForSeries15i(handle);
            }
            else
            {
                return GetStateInfoForSeries16i18i21i30i31i32i35i0i(handle);
            }
        }

        /// <summary>
        /// 取得 CNC 系統類型
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private static Focas1.ODBSYS_CNC_TYPE GetMachineType(ushort handle)
        {
            var systemInfo = new Focas1.ODBSYS();
            EnsureOk(Focas1.cnc_sysinfo(handle, systemInfo));
            return Focas1.cnc_type_convert(systemInfo.cnc_type);
        }

        /// <summary>
        /// 取得 15i 系列的狀態資訊
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private static FocasStateInfo GetStateInfoForSeries15i(ushort handle)
        {
            var stateInfo = new Focas1.ODBST();
            EnsureOk(Focas1.cnc_statinfo(handle, stateInfo));
            var isEmergency = stateInfo.emergency switch
            {
                1 => true,
                _ => false
            };
            var isAlarm = stateInfo.alarm switch
            {
                1 => true,
                _ => false
            };
            var runStatus = stateInfo.run switch
            {
                0 => FocasRunStatus.Stop,
                1 => FocasRunStatus.Hold,
                >= 2 and <= 7 => FocasRunStatus.Start,
                8 => FocasRunStatus.Reset,
                _ => FocasRunStatus.Unknown
            };
            var operatingMode = stateInfo.aut switch
            {
                0 => "****",
                1 => "MDI",
                2 => "DNC",
                3 => "MEM",
                4 => "EDIT",
                5 => "Teach in",
                _ => ""
            };
            return new FocasStateInfo
            {
                RunStatus = isEmergency || isAlarm ? FocasRunStatus.Alarm : runStatus,
                OperatingMode = operatingMode
            };
        }

        /// <summary>
        /// 取得 16i/18i/21i/30i/31i/32i/35i/0i 系列的狀態資訊
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private static FocasStateInfo GetStateInfoForSeries16i18i21i30i31i32i35i0i(ushort handle)
        {
            var stateInfo = new Focas1.ODBST();
            EnsureOk(Focas1.cnc_statinfo(handle, stateInfo));
            var isEmergency = stateInfo.emergency switch
            {
                1 => true,
                _ => false
            };
            var isAlarm = stateInfo.alarm switch
            {
                0 => false, // ***(Others)
                2 => false, // Battery low
                3 => false, // NC or Servo amplifier
                4 => false, // PS warning
                5 => false, // FSsB warning
                6 => false, // Insulate warning
                7 => false, // Encoder warning
                _ => true
            };
            var runStatus = stateInfo.run switch
            {
                0 => FocasRunStatus.Reset,
                1 => FocasRunStatus.Stop,
                2 => FocasRunStatus.Hold,
                3 or 4 => FocasRunStatus.Start,
                _ => FocasRunStatus.Unknown
            };
            var operatingMode = stateInfo.aut switch
            {
                0 => "MDI",
                1 => "MEM",
                2 => "****",
                3 => "EDIT",
                4 => "HANDLE",
                5 => "JOG",
                6 => "Teach in JOG",
                7 => "Teach in HANDLE",
                8 => "INC.FEED",
                9 => "REF",
                10 => "REMOTE",
                _ => ""
            };
            return new FocasStateInfo
            {
                RunStatus = isEmergency || isAlarm ? FocasRunStatus.Alarm : runStatus,
                OperatingMode = operatingMode
            };
        }

        /// <summary>
        /// 取得警報資訊
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static FocasAlarmInfo GetAlarmInfo(ushort handle)
        {
            var machineType = GetMachineType(handle);
            if (machineType == Focas1.ODBSYS_CNC_TYPE.S30 ||
                machineType == Focas1.ODBSYS_CNC_TYPE.S31 ||
                machineType == Focas1.ODBSYS_CNC_TYPE.S32 ||
                machineType == Focas1.ODBSYS_CNC_TYPE.S35 ||
                machineType == Focas1.ODBSYS_CNC_TYPE.S0)
            {
                return GetAlarmInfoForSeries30i31i32i35i0i(handle);
            }
            else
            {
                return GetAlarmInfoForSeries15i16i18i21i(handle);
            }
        }

        /// <summary>
        /// 取得 30i/31i/32i/35i/0i 系列的警報資訊
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private static FocasAlarmInfo GetAlarmInfoForSeries30i31i32i35i0i(ushort handle)
        {
            var alarmInfo = new Focas1.ODBALMMSG2();
            var allAlarmTypeToRead = (short)-1;
            var alarmInfoDataNumber = Focas1.ODBALMMSG2_DATA_NUMBER;
            EnsureOk(Focas1.cnc_rdalmmsg2(handle, allAlarmTypeToRead, ref alarmInfoDataNumber, alarmInfo));
            return new FocasAlarmInfo
            {
                AlarmMessages = Enumerable.Range(1, alarmInfoDataNumber)
                                          .Select(index =>
                                          {
                                              var alarmData = alarmInfo.GetType().GetField($"msg{index}")?.GetValue(alarmInfo) as Focas1.ODBALMMSG2_data;
                                              return new FocasAlarmMessage
                                              {
                                                  No = alarmData.alm_no,
                                                  Message = Encoding.Default.GetString(Encoding.Default.GetBytes(alarmData.alm_msg))
                                              };
                                          })
                                          .ToList()
            };
        }

        /// <summary>
        /// 取得 15i/16i/18i/21i 系列的警報資訊
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private static FocasAlarmInfo GetAlarmInfoForSeries15i16i18i21i(ushort handle)
        {
            var alarmInfo = new Focas1.ODBALMMSG();
            var allAlarmTypeToRead = (short)-1;
            var alarmInfoDataNumber = Focas1.ODBALMMSG_DATA_NUMBER;
            EnsureOk(Focas1.cnc_rdalmmsg(handle, allAlarmTypeToRead, ref alarmInfoDataNumber, alarmInfo));
            return new FocasAlarmInfo
            {
                AlarmMessages = Enumerable.Range(1, alarmInfoDataNumber)
                                          .Select(index =>
                                          {
                                              var alarmData = alarmInfo.GetType().GetField($"msg{index}")?.GetValue(alarmInfo) as Focas1.ODBALMMSG_data;
                                              return new FocasAlarmMessage
                                              {
                                                  No = alarmData.alm_no,
                                                  Message = Encoding.Default.GetString(Encoding.Default.GetBytes(alarmData.alm_msg))
                                              };
                                          })
                                          .ToList()
            };
        }

        /// <summary>
        /// 取得程式資訊
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static FocasProgramInfo GetProgramInfo(ushort handle)
        {
            var programNoInfo = new Focas1.ODBPRO();
            EnsureOk(Focas1.cnc_rdprgnum(handle, programNoInfo));
            return new FocasProgramInfo
            {
                MainProgramName = GetProgramName(programNoInfo.mdata),
                ExecutingProgramName = GetProgramName(programNoInfo.data)
            };
        }

        /// <summary>
        /// 將程式號轉換為 Oxxx 格式的程式名稱
        /// </summary>
        /// <param name="programNo"></param>
        /// <returns></returns>
        private static string GetProgramName(short programNo)
        {
            return string.Format("O{0:d4}", programNo);
        }

        /// <summary>
        /// 取得主軸資訊
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="pmcSetting"></param>
        /// <returns></returns>
        public static FocasSpindleInfo GetSpindleInfo(ushort handle)
        {
            // 獲取主軸轉速與進給率
            var speedInfo = new Focas1.ODBSPEED();
            var allTypeToRead = (short)-1;
            EnsureOk(Focas1.cnc_rdspeed(handle, allTypeToRead, speedInfo));
            var actualFeedrate = (int)(speedInfo.actf.data * Math.Pow(0.1, speedInfo.actf.dec));
            var actualSpeed = (int)(speedInfo.acts.data * Math.Pow(0.1, speedInfo.acts.dec));

            // 獲取進給率百分比 (透過讀取PMC)
            var feedratePercentageRaw = ReadPMCRawValueAsInt(handle, Focas1.PMC_ADR_TYPE.G, 12, Focas1.PMC_DATA_TYPE.Word);
            var feedratePercentage = ConvertFeedratePercentage(feedratePercentageRaw);

            // 獲取快速進給率百分比 (透過讀取PMC)
            var feedrateRapidPercentageRaw = ReadPMCRawValueAsInt(handle, Focas1.PMC_ADR_TYPE.G, 14, Focas1.PMC_DATA_TYPE.Byte);
            var feedrateRapidPercentage = ConvertFeedrateRapidPercentage(feedrateRapidPercentageRaw);

            // 獲取主軸轉速百分比 (透過讀取PMC)
            var speedPercentageRaw = ReadPMCRawValueAsInt(handle, Focas1.PMC_ADR_TYPE.G, 30, Focas1.PMC_DATA_TYPE.Word);
            var speedPercentage = ConvertSpeedPercentage(speedPercentageRaw);

            // 獲取主軸負載百分比
            var spindleLoadInfo = new Focas1.ODBSPLOAD();
            var spindleLoadData = (short)0;
            var spindleLoadInfoDataNumber = Focas1.ODBSPLOAD_DATA_NUMBER;
            EnsureOk(Focas1.cnc_rdspmeter(handle, spindleLoadData, ref spindleLoadInfoDataNumber, spindleLoadInfo));
            var loadPercentage = Convert.ToInt32(spindleLoadInfo.spload1.spload.data * Math.Pow(0.1, spindleLoadInfo.spload1.spload.dec));

            return new FocasSpindleInfo
            {
                FeedRateInfo = new FocasFeedRateInfo
                {
                    Actual = actualFeedrate,
                    Percentage = feedratePercentage,
                    RapidPercentage = feedrateRapidPercentage
                },
                SpeedInfo = new FocasSpeedInfo
                {
                    Actual = actualSpeed,
                    Percentage = speedPercentage
                },
                LoadInfo = new FocasLoadInfo
                {
                    Percentage = loadPercentage
                }
            };
        }

        /// <summary>
        /// 從 PMC 讀取原始值
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="readSetting"></param>
        /// <returns></returns>
        private static int ReadPMCRawValueAsInt(ushort handle, Focas1.PMC_ADR_TYPE addressType, int addressIndex, Focas1.PMC_DATA_TYPE addressDataType)
        {
            return addressDataType switch
            {
                Focas1.PMC_DATA_TYPE.Byte =>
                    ReadPMCByteData(handle, addressType, addressIndex),

                Focas1.PMC_DATA_TYPE.Word =>
                    ReadPMCWordData(handle, addressType, addressIndex),

                Focas1.PMC_DATA_TYPE.Long =>
                    ReadPMCLongData(handle, addressType, addressIndex),

                _ => 0
            };
        }

        /// <summary>
        /// 讀取 PMC Byte 型資料
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="addressType"></param>
        /// <param name="addressIndex"></param>
        /// <returns></returns>
        private static byte ReadPMCByteData(ushort handle, Focas1.PMC_ADR_TYPE addressType, int addressIndex)
        {
            var pmcInfo = new Focas1.IODBPMC0();
            EnsureOk(Focas1.pmc_rdpmcrng(handle, (short)addressType, (short)Focas1.PMC_DATA_TYPE.Byte, (ushort)addressIndex, (ushort)addressIndex, Focas1.IODBPMC0_LENGTH, pmcInfo));
            return pmcInfo.cdata[0];
        }

        /// <summary>
        /// 讀取 PMC Word 型資料
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="addressType"></param>
        /// <param name="addressIndex"></param>
        /// <returns></returns>
        private static short ReadPMCWordData(ushort handle, Focas1.PMC_ADR_TYPE addressType, int addressIndex)
        {
            var pmcInfo = new Focas1.IODBPMC1();
            EnsureOk(Focas1.pmc_rdpmcrng(handle, (short)addressType, (short)Focas1.PMC_DATA_TYPE.Word, (ushort)addressIndex, (ushort)addressIndex, Focas1.IODBPMC1_LENGTH, pmcInfo));
            return pmcInfo.idata[0];
        }

        /// <summary>
        /// 讀取 PMC Long 型資料
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="addressType"></param>
        /// <param name="addressIndex"></param>
        /// <returns></returns>
        private static int ReadPMCLongData(ushort handle, Focas1.PMC_ADR_TYPE addressType, int addressIndex)
        {
            var pmcInfo = new Focas1.IODBPMC2();
            EnsureOk(Focas1.pmc_rdpmcrng(handle, (short)addressType, (short)Focas1.PMC_DATA_TYPE.Long, (ushort)addressIndex, (ushort)addressIndex, Focas1.IODBPMC2_LENGTH, pmcInfo));
            return pmcInfo.ldata[0];
        }

        /// <summary>
        /// 將 PMC 原始值轉換為進給率百分比
        /// </summary>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        private static int ConvertFeedratePercentage(int rawValue)
        {
            return rawValue switch
            {
                < 0 => (rawValue ^ 0xff) & 0x00ff,
                _ when rawValue % 10 == 0 => rawValue & 0x00ff,
                _ => rawValue ^ 0x00ff
            };
        }

        /// <summary>
        /// 將 PMC 原始值轉換為快速進給率百分比
        /// </summary>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        private static int ConvertFeedrateRapidPercentage(int rawValue)
        {
            return rawValue switch
            {
                0 => 100,
                1 => 50,
                2 => 25,
                3 => 0,
                _ => 0
            };
        }

        /// <summary>
        /// 將 PMC 原始值轉換為主軸轉速百分比
        /// </summary>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        private static int ConvertSpeedPercentage(int rawValue)
        {
            return rawValue;
        }

        /// <summary>
        /// 取得位置資訊
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static FocasPositionInfo GetPositionInfo(ushort handle)
        {
            var positionInfo = new Focas1.ODBPOS();
            var allTypeToRead = (short)-1;
            var positionDataCount = Focas1.ODBPOS_DATA_NUMBER;
            EnsureOk(Focas1.cnc_rdposition(handle, allTypeToRead, ref positionDataCount, positionInfo));
            var positionDatum = GetPositionDatum(positionInfo, positionDataCount);
            var machinePosition = positionDatum.ToDictionary(data => GetPositionElementName(data.mach), data => GetPositionElementValue(data.mach));
            var absolutePosition = positionDatum.ToDictionary(data => GetPositionElementName(data.abs), data => GetPositionElementValue(data.abs));
            var relativePosition = positionDatum.ToDictionary(data => GetPositionElementName(data.rel), data => GetPositionElementValue(data.rel));
            var distanceToGo = positionDatum.ToDictionary(data => GetPositionElementName(data.dist), data => GetPositionElementValue(data.dist));
            return new FocasPositionInfo
            {
                MachinePosition = machinePosition,
                AbsolutePosition = absolutePosition,
                RelativePosition = relativePosition,
                DistanceToGo = distanceToGo
            };
        }

        /// <summary>
        /// 從原始位置結構中擷取所有軸資料
        /// </summary>
        /// <param name="positionInfo"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static List<Focas1.POSELMALL> GetPositionDatum(Focas1.ODBPOS positionInfo, int count)
        {
            var positionDatum = new List<Focas1.POSELMALL>(count);

            var type = typeof(Focas1.ODBPOS);
            for (int i = 1; i <= count; i++)
            {
                var field = type.GetField($"p{i}");
                if (field?.GetValue(positionInfo) is Focas1.POSELMALL positionData)
                {
                    positionDatum.Add(positionData);
                }
            }

            return positionDatum;
        }

        /// <summary>
        /// 取得單一軸的名稱
        /// </summary>
        /// <param name="positionElement"></param>
        /// <returns></returns>
        private static string GetPositionElementName(Focas1.POSELM positionElement)
        {
            return positionElement.name.ToString();
        }

        /// <summary>
        /// 取得單一軸的位置數值
        /// </summary>
        /// <param name="positionElement"></param>
        /// <returns></returns>
        private static double GetPositionElementValue(Focas1.POSELM positionElement)
        {
            return positionElement.data * Math.Pow(0.1, positionElement.dec);
        }

        /// <summary>
        /// 驗證 FOCAS 呼叫結果碼
        /// </summary>
        /// <param name="resultCode"></param>
        /// <exception cref="FocasException"></exception>
        public static void EnsureOk(short resultCode)
        {
            if (resultCode != Focas1.EW_OK)
                throw new FocasException(resultCode);
        }
    }
}