using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.DeviceDrivers;
using IIoTHub.Domain.Models.DeviceSettings;
using IIoTHub.Domain.Models.DeviceSnapshots;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Enums;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Models;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Requests;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.Focas.Responses;
using IIoTHub.Infrastructure.DeviceDriverHosts;
using IIoTHub.Infrastructure.DeviceDriverHosts.Interfaces;
using IIoTHub.Infrastructure.DeviceDriverHosts.Models;
using IIoTHub.Infrastructure.DeviceDrivers.Attributes;

namespace IIoTHub.Infrastructure.DeviceDrivers.Drivers.Machines
{
    public class FanucFocasLibraryDriver : IMachineDriver
    {
        public string Name => "FANUC FOCAS LIBRARY 驅動器";

        #region 參數設定

        [ParameterSetting("DriverHost可執行檔完整路徑", "可執行檔 IIoTHub.Infrastructure.DeviceDriverHost.Focas.x86.exe 所在的完整路徑", "")]
        public string DriverHostExeFilePath { get; set; }

        [ParameterSetting("IP 位址","", "127.0.0.1")]
        public string IpAddress { get; set; } = "127.0.0.1";

        [ParameterSetting("通訊埠", "預設: 8193", "8193")]
        public int Port { get; set; } = 8193;

        [ParameterSetting("超時時間", "單位: 秒", "10")]
        public int Timeout { get; set; } = 10;

        [ParameterSetting("進給倍率 PMC 位址型別", "", "G")]
        public FocasPMCAddressType FeedratePercentageAddressType { get; set; } = FocasPMCAddressType.G;

        [ParameterSetting("進給倍率 PMC 資料型別", "", "Word")]
        public FocasPMCAddressDataType FeedratePercentageAddressDataType { get; set; } = FocasPMCAddressDataType.Word;

        [ParameterSetting("進給倍率 PMC 位址索引", "", "12")]
        public int FeedratePercentageAddressIndex { get; set; } = 12;

        [ParameterSetting("快速進給倍率 PMC 位址型別", "", "G")]
        public FocasPMCAddressType FeedrateRapidPercentageAddressType { get; set; } = FocasPMCAddressType.G;

        [ParameterSetting("快速進給倍率 PMC 資料型別", "", "Byte")]
        public FocasPMCAddressDataType FeedrateRapidPercentageAddressDataType { get; set; } = FocasPMCAddressDataType.Byte;

        [ParameterSetting("快速進給倍率 PMC 位址索引", "", "14")]
        public int FeedrateRapidPercentageAddressIndex { get; set; } = 14;

        [ParameterSetting("主軸轉速倍率 PMC 位址型別", "", "G")]
        public FocasPMCAddressType SpeedPercentageAddressType { get; set; } = FocasPMCAddressType.G;

        [ParameterSetting("主軸轉速倍率 PMC 資料型別", "", "Word")]
        public FocasPMCAddressDataType SpeedPercentageAddressDataType { get; set; } = FocasPMCAddressDataType.Word;

        [ParameterSetting("主軸轉速倍率 PMC 位址索引", "", "30")]
        public int SpeedPercentageAddressIndex { get; set; } = 30;

        #endregion

        #region 實作

        private readonly IDeviceDriverHostManager _deviceDriverHostManager;

        public FanucFocasLibraryDriver(IDeviceDriverHostManager deviceDriverHostManager)
        {
            _deviceDriverHostManager = deviceDriverHostManager;
        }

        /// <summary>
        /// 取得指定設備的快照
        /// </summary>
        /// <param name="deviceSetting"></param>
        /// <returns></returns>
        public MachineSnapshot GetSnapshot(DeviceSetting deviceSetting)
        {
            try
            {
                var parameters = deviceSetting.DriverSetting.ParameterSettings
                    .ToDictionary(v => v.Key, v => v.Value);

                var driverHostExeFilePath = GetDriverHostExeFilePath(parameters);
                _deviceDriverHostManager.EnsureRunning(new DeviceDriverHostDescriptor(FocasPipeNames.DriverHostX86, driverHostExeFilePath));

                TResponse SendRequest<TRequest, TResponse>(TRequest request) where TResponse : FocasBaseInfoResponse, new()
                {
                    var response = DeviceDriverHostClient.Send<TRequest, TResponse>(FocasPipeNames.DriverHostX86, request);
                    if (!response.IsSuccess)
                        throw new InvalidOperationException("DriverHost回傳執行失敗!");
                    return response;
                }

                (var ipAddress, var port, var timeout) = GetConnectionSettings(parameters);
                var stateInfoResponse = SendRequest<FocasStateInfoRequest, FocasStateInfoResponse>(new FocasStateInfoRequest
                {
                    IpAddress = ipAddress,
                    Port = port,
                    Timeout = timeout
                });
                var programInfoResponse = SendRequest<FocasProgramInfoRequest, FocasProgramInfoResponse>(new FocasProgramInfoRequest
                {
                    IpAddress = ipAddress,
                    Port = port,
                    Timeout = timeout
                });
                var spindleInfoResponse = SendRequest<FocasSpindleInfoRequest, FocasSpindleInfoResponse>(new FocasSpindleInfoRequest
                {
                    IpAddress = ipAddress,
                    Port = port,
                    Timeout = timeout,
                    SpindlePMCReadSetting = GetSpindlePMCReadSetting(parameters)
                });
                var positionInfoResponse = SendRequest<FocasPositionInfoRequest, FocasPositionInfoResponse>(new FocasPositionInfoRequest
                {
                    IpAddress = ipAddress,
                    Port = port,
                    Timeout = timeout
                });

                return new MachineSnapshot(
                    deviceSetting.Id,
                    stateInfoResponse.StateInfo.RunStatus switch
                    {
                        FocasRunStatus.Reset => DeviceRunStatus.Standby,
                        FocasRunStatus.Stop => DeviceRunStatus.Running,
                        FocasRunStatus.Hold => DeviceRunStatus.Running,
                        FocasRunStatus.Alarm => DeviceRunStatus.Alarm,
                        _ => DeviceRunStatus.Offline
                    },
                    stateInfoResponse.StateInfo.OperatingMode,
                    programInfoResponse.ProgramInfo.MainProgramName,
                    programInfoResponse.ProgramInfo.ExecutingProgramName,
                    spindleInfoResponse.SpindleInfo.FeedRateInfo.Actual,
                    spindleInfoResponse.SpindleInfo.FeedRateInfo.Percentage,
                    spindleInfoResponse.SpindleInfo.FeedRateInfo.RapidPercentage,
                    spindleInfoResponse.SpindleInfo.SpeedInfo.Actual,
                    spindleInfoResponse.SpindleInfo.SpeedInfo.Percentage,
                    spindleInfoResponse.SpindleInfo.LoadInfo.Percentage,
                    positionInfoResponse.PositionInfo.MachinePosition,
                    positionInfoResponse.PositionInfo.AbsolutePosition,
                    positionInfoResponse.PositionInfo.RelativePosition,
                    positionInfoResponse.PositionInfo.DistanceToGo
                    );
            }
            catch (Exception)
            {
                return MachineSnapshot.OfflineSnapshot(deviceSetting.Id);
            }
        }

        /// <summary>
        /// 取得驅動器宿主可執行檔完整路徑
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private string GetDriverHostExeFilePath(Dictionary<string, string> parameters)
        {
            var exeFilePath = DriverShared.GetString(parameters, nameof(DriverHostExeFilePath), DriverHostExeFilePath);
            return exeFilePath;
        }

        /// <summary>
        /// 取得連線設定
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private (string Ip, int Port, int Timeout) GetConnectionSettings(Dictionary<string, string> parameters)
        {
            var ipAddress = DriverShared.GetString(parameters, nameof(IpAddress), IpAddress);
            var port = DriverShared.GetInt(parameters, nameof(Port), Port);
            var timeout = DriverShared.GetInt(parameters, nameof(Timeout), Timeout);
            return (ipAddress, port, timeout);
        }

        /// <summary>
        /// 取得主軸相關 PMC 讀取設定
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private FocasSpindlePMCReadSetting GetSpindlePMCReadSetting(Dictionary<string, string> parameters)
        {
            var feedratePercentageAddressType = DriverShared.GetEnum(parameters, nameof(FeedratePercentageAddressType), FeedratePercentageAddressType);
            var feedratePercentageAddressDataType = DriverShared.GetEnum(parameters, nameof(FeedratePercentageAddressDataType), FeedratePercentageAddressDataType);
            var feedratePercentageAddressIndex = DriverShared.GetInt(parameters, nameof(FeedratePercentageAddressIndex), FeedratePercentageAddressIndex);
            var feedrateRapidPercentageAddressType = DriverShared.GetEnum(parameters, nameof(FeedrateRapidPercentageAddressType), FeedrateRapidPercentageAddressType);
            var feedrateRapidPercentageAddressDataType = DriverShared.GetEnum(parameters, nameof(FeedrateRapidPercentageAddressDataType), FeedrateRapidPercentageAddressDataType);
            var feedrateRapidPercentageAddressIndex = DriverShared.GetInt(parameters, nameof(FeedrateRapidPercentageAddressIndex), FeedrateRapidPercentageAddressIndex);
            var speedPercentageAddressType = DriverShared.GetEnum(parameters, nameof(SpeedPercentageAddressType), SpeedPercentageAddressType);
            var speedPercentageAddressDataType = DriverShared.GetEnum(parameters, nameof(SpeedPercentageAddressDataType), SpeedPercentageAddressDataType);
            var speedPercentageAddressIndex = DriverShared.GetInt(parameters, nameof(SpeedPercentageAddressIndex), SpeedPercentageAddressIndex);
            return new FocasSpindlePMCReadSetting(
                new FocasPMCReadSetting(feedratePercentageAddressType, feedratePercentageAddressDataType, feedratePercentageAddressIndex),
                new FocasPMCReadSetting(feedrateRapidPercentageAddressType, feedrateRapidPercentageAddressDataType, feedrateRapidPercentageAddressIndex),
                new FocasPMCReadSetting(speedPercentageAddressType, speedPercentageAddressDataType, speedPercentageAddressIndex));
        }

        #endregion
    }
}
