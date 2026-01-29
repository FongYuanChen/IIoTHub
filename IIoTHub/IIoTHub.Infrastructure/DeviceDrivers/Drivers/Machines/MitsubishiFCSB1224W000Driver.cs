using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.DeviceDrivers;
using IIoTHub.Domain.Models.DeviceSettings;
using IIoTHub.Domain.Models.DeviceSnapshots;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Enums;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Requests;
using IIoTHub.Infrastructure.DeviceDriverHost.Abstractions.FCSB1224W000.Responses;
using IIoTHub.Infrastructure.DeviceDriverHosts;
using IIoTHub.Infrastructure.DeviceDriverHosts.Interfaces;
using IIoTHub.Infrastructure.DeviceDriverHosts.Models;
using IIoTHub.Infrastructure.DeviceDrivers.Attributes;

namespace IIoTHub.Infrastructure.DeviceDrivers.Drivers.Machines
{
    public class MitsubishiFCSB1224W000Driver : IMachineDriver
    {
        public string Name => "Mitsubishi FCSB1224W000 驅動器";

        #region 參數設定

        [ParameterSetting("DriverHost可執行檔完整路徑", "可執行檔 IIoTHub.Infrastructure.DeviceDriverHost.FCSB1224W000.x86.exe 所在的完整路徑", "")]
        public string DriverHostExeFilePath { get; set; }

        [ParameterSetting("系統類型", "", "")]
        public FCSB1224W000SystemType SystemType { get; set; } = FCSB1224W000SystemType.M800M;

        [ParameterSetting("IP 位址", "", "127.0.0.1")]
        public string IpAddress { get; set; } = "127.0.0.1";

        [ParameterSetting("通訊埠", "預設: 683", "683")]
        public int Port { get; set; } = 683;

        [ParameterSetting("超時時間", "單位: 秒", "10")]
        public int Timeout { get; set; } = 10;

        #endregion

        #region 實作

        private readonly IDeviceDriverHostManager _deviceDriverHostManager;

        public MitsubishiFCSB1224W000Driver(IDeviceDriverHostManager deviceDriverHostManager)
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
                _deviceDriverHostManager.EnsureRunning(new DeviceDriverHostDescriptor(FCSB1224W000PipeNames.DriverHostX86, driverHostExeFilePath));

                TResponse SendRequest<TRequest, TResponse>(TRequest request) where TResponse : FCSB1224W000BaseInfoResponse, new()
                {
                    var response = DeviceDriverHostClient.Send<TRequest, TResponse>(FCSB1224W000PipeNames.DriverHostX86, request);
                    if (!response.IsSuccess)
                        throw new InvalidOperationException("DriverHost回傳執行失敗!");
                    return response;
                }

                (var systemType, var ipAddress, var port, var timeout) = GetConnectionSettings(parameters);
                var stateInfoResponse = SendRequest<FCSB1224W000StateInfoRequest, FCSB1224W000StateInfoResponse>(new FCSB1224W000StateInfoRequest
                {
                    SystemType = systemType,
                    IpAddress = ipAddress,
                    Port = port,
                    Timeout = timeout
                });
                var programInfoResponse = SendRequest<FCSB1224W000ProgramInfoRequest, FCSB1224W000ProgramInfoResponse>(new FCSB1224W000ProgramInfoRequest
                {
                    SystemType = systemType,
                    IpAddress = ipAddress,
                    Port = port,
                    Timeout = timeout
                });
                var spindleInfoResponse = SendRequest<FCSB1224W000SpindleInfoRequest, FCSB1224W000SpindleInfoResponse>(new FCSB1224W000SpindleInfoRequest
                {
                    SystemType = systemType,
                    IpAddress = ipAddress,
                    Port = port,
                    Timeout = timeout
                });
                var positionInfoResponse = SendRequest<FCSB1224W000PositionInfoRequest, FCSB1224W000PositionInfoResponse>(new FCSB1224W000PositionInfoRequest
                {
                    SystemType = systemType,
                    IpAddress = ipAddress,
                    Port = port,
                    Timeout = timeout
                });

                return new MachineSnapshot(
                    deviceSetting.Id,
                    stateInfoResponse.StateInfo.RunStatus switch
                    {
                        FCSB1224W000RunStatus.Reset => DeviceRunStatus.Standby,
                        FCSB1224W000RunStatus.Stop => DeviceRunStatus.Running,
                        FCSB1224W000RunStatus.Hold => DeviceRunStatus.Running,
                        FCSB1224W000RunStatus.Alarm => DeviceRunStatus.Alarm,
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
                    [],
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
        private (FCSB1224W000SystemType SystemType, string Ip, int Port, int Timeout) GetConnectionSettings(Dictionary<string, string> parameters)
        {
            var systemType = DriverShared.GetEnum(parameters, nameof(SystemType), SystemType);
            var ipAddress = DriverShared.GetString(parameters, nameof(IpAddress), IpAddress);
            var port = DriverShared.GetInt(parameters, nameof(Port), Port);
            var timeout = DriverShared.GetInt(parameters, nameof(Timeout), Timeout);
            return (systemType, ipAddress, port, timeout);
        }

        #endregion
    }
}
