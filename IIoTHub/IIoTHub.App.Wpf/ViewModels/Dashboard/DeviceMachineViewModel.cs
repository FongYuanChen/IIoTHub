using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Models.DeviceRecords;
using IIoTHub.Domain.Models.DeviceSettings;
using IIoTHub.Domain.Models.DeviceSnapshots;

namespace IIoTHub.App.Wpf.ViewModels.Dashboard
{
    /// <summary>
    /// 機台設備的 ViewModel
    /// </summary>
    public class DeviceMachineViewModel : DeviceViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IDeviceSettingService _deviceService;
        private readonly IDeviceSnapshotMonitorService _deviceSnapshotMonitorService;
        private readonly DeviceSetting _deviceSetting;
        private readonly IDeviceRuntimeStatisticsService _deviceRuntimeStatisticsService;
        private readonly IDeviceSnapshotPublisher _deviceSnapshotPublisher;
        private readonly Action _snapshotUpdatedAction;
        private readonly Action<DeviceViewModelBase> _showDetailAction;

        private IDisposable _subscriptionDisposable;
        private string _operatingMode;
        private string _mainProgramName;
        private string _executingProgramName;
        private int _feedRateActual;
        private int _feedRatePercentage;
        private int _feedRateRapidPercentage;
        private int _speedActual;
        private int _speedPercentage;
        private int _loadPercentage;
        private Dictionary<string, double> _machinePosition;
        private Dictionary<string, double> _absolutePosition;
        private Dictionary<string, double> _relativePosition;
        private Dictionary<string, double> _distanceToGo;
        private List<DeviceRuntimeRecord> _runtimeRecords;
        private double _offlineUtilization;
        private double _standbyUtilization;
        private double _runningUtilization;
        private double _alarmUtilization;

        private DeviceMachineViewModel(IDialogService dialogService,
                                       IDeviceSettingService deviceService,
                                       IDeviceSnapshotMonitorService deviceSnapshotMonitorService,
                                       DeviceSetting deviceSetting,
                                       IDeviceRuntimeStatisticsService deviceRuntimeStatisticsService,
                                       IDeviceSnapshotPublisher deviceSnapshotPublisher,
                                       Action snapshotUpdatedAction,
                                       Action<DeviceViewModelBase> showDetailAction) : base(dialogService, deviceService, deviceSnapshotMonitorService, deviceSetting)
        {
            _dialogService = dialogService;
            _deviceService = deviceService;
            _deviceSnapshotMonitorService = deviceSnapshotMonitorService;
            _deviceSetting = deviceSetting;
            _deviceRuntimeStatisticsService = deviceRuntimeStatisticsService;
            _deviceSnapshotPublisher = deviceSnapshotPublisher;
            _snapshotUpdatedAction = snapshotUpdatedAction;
            _showDetailAction = showDetailAction;
        }

        /// <summary>
        /// 建立 DeviceMachineViewModel
        /// </summary>
        /// <param name="dialogService"></param>
        /// <param name="deviceService"></param>
        /// <param name="deviceSnapshotMonitorService"></param>
        /// <param name="deviceSetting"></param>
        /// <param name="deviceRuntimeStatisticsService"></param>
        /// <param name="deviceSnapshotPublisher"></param>
        /// <param name="snapshotUpdatedAction"></param>
        /// <param name="showDetailAction"></param>
        /// <returns></returns>
        public static async Task<DeviceMachineViewModel> CreateAsync(IDialogService dialogService,
                                                                     IDeviceSettingService deviceService,
                                                                     IDeviceSnapshotMonitorService deviceSnapshotMonitorService,
                                                                     DeviceSetting deviceSetting,
                                                                     IDeviceRuntimeStatisticsService deviceRuntimeStatisticsService,
                                                                     IDeviceSnapshotPublisher deviceSnapshotPublisher,
                                                                     Action snapshotUpdatedAction,
                                                                     Action<DeviceViewModelBase> showDetailAction)
        {
            var deviceViewModel = new DeviceMachineViewModel(dialogService,
                                                             deviceService,
                                                             deviceSnapshotMonitorService,
                                                             deviceSetting,
                                                             deviceRuntimeStatisticsService,
                                                             deviceSnapshotPublisher,
                                                             snapshotUpdatedAction,
                                                             showDetailAction);
            await deviceViewModel.InitializeAsync();
            return deviceViewModel;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            // 訂閱設備運轉統計摘要變化事件
            _deviceRuntimeStatisticsService.DeviceRuntimeSummaryChanged += (s, e) =>
            {
                RuntimeRecords = e.DeviceRuntimeSummary.RuntimeRecords;
                OfflineUtilization = e.DeviceRuntimeSummary.OfflineUtilization;
                StandbyUtilization = e.DeviceRuntimeSummary.StandbyUtilization;
                RunningUtilization = e.DeviceRuntimeSummary.RunningUtilization;
                AlarmUtilization = e.DeviceRuntimeSummary.AlarmUtilization;
                Utilization = e.DeviceRuntimeSummary.RunningUtilization;
            };

            // 訂閱設備快照更新
            _subscriptionDisposable = _deviceSnapshotPublisher.Subscribe<MachineSnapshot>(_deviceSetting.Id, snapshot =>
            {
                Status = snapshot.RunStatus;
                OperatingMode = snapshot.OperatingMode;
                MainProgramName = snapshot.MainProgramName;
                ExecutingProgramName = snapshot.ExecutingProgramName;
                FeedRateActual = snapshot.FeedRateActual;
                FeedRatePercentage = snapshot.FeedRatePercentage;
                FeedRateRapidPercentage = snapshot.FeedRateRapidPercentage;
                SpeedActual = snapshot.SpeedActual;
                SpeedPercentage = snapshot.SpeedPercentage;
                LoadPercentage = snapshot.LoadPercentage;
                MachinePosition = snapshot.MachinePosition.ToDictionary();
                AbsolutePosition = snapshot.AbsolutePosition.ToDictionary();
                RelativePosition = snapshot.RelativePosition.ToDictionary();
                DistanceToGo = snapshot.DistanceToGo.ToDictionary();
                _snapshotUpdatedAction();
            });
        }

        /// <summary>
        /// 操作模式
        /// </summary>
        public string OperatingMode
        {
            get => _operatingMode;
            set
            {
                if (_operatingMode == value)
                    return;

                _operatingMode = value;

                OnPropertyChanged(nameof(OperatingMode));
            }
        }

        /// <summary>
        /// 主程式
        /// </summary>
        public string MainProgramName
        {
            get => _mainProgramName;
            set
            {
                if (_mainProgramName == value)
                    return;

                _mainProgramName = value;

                OnPropertyChanged(nameof(MainProgramName));
            }
        }

        /// <summary>
        /// 執行程式
        /// </summary>
        public string ExecutingProgramName
        {
            get => _executingProgramName;
            set
            {
                if (_executingProgramName == value)
                    return;

                _executingProgramName = value;

                OnPropertyChanged(nameof(ExecutingProgramName));
            }
        }

        /// <summary>
        /// 主軸進給
        /// </summary>
        public int FeedRateActual
        {
            get => _feedRateActual;
            set
            {
                if (_feedRateActual == value)
                    return;

                _feedRateActual = value;

                OnPropertyChanged(nameof(FeedRateActual));
            }
        }

        /// <summary>
        /// 主軸進給倍率
        /// </summary>
        public int FeedRatePercentage
        {
            get => _feedRatePercentage;
            set
            {
                if (_feedRatePercentage == value)
                    return;

                _feedRatePercentage = value;

                OnPropertyChanged(nameof(FeedRatePercentage));
            }
        }

        /// <summary>
        /// 主軸快速進給倍率
        /// </summary>
        public int FeedRateRapidPercentage
        {
            get => _feedRateRapidPercentage;
            set
            {
                if (_feedRateRapidPercentage == value)
                    return;

                _feedRateRapidPercentage = value;

                OnPropertyChanged(nameof(FeedRateRapidPercentage));
            }
        }

        /// <summary>
        /// 主軸轉速
        /// </summary>
        public int SpeedActual
        {
            get => _speedActual;
            set
            {
                if (_speedActual == value)
                    return;

                _speedActual = value;

                OnPropertyChanged(nameof(SpeedActual));
            }
        }

        /// <summary>
        /// 主軸轉速倍率
        /// </summary>
        public int SpeedPercentage
        {
            get => _speedPercentage;
            set
            {
                if (_speedPercentage == value)
                    return;

                _speedPercentage = value;

                OnPropertyChanged(nameof(SpeedPercentage));
            }
        }

        /// <summary>
        /// 主軸負載
        /// </summary>
        public int LoadPercentage
        {
            get => _loadPercentage;
            set
            {
                if (_loadPercentage == value)
                    return;

                _loadPercentage = value;

                OnPropertyChanged(nameof(LoadPercentage));
            }
        }

        /// <summary>
        /// 機械座標
        /// </summary>
        public Dictionary<string, double> MachinePosition
        {
            get => _machinePosition;
            set
            {
                _machinePosition = value;

                OnPropertyChanged(nameof(MachinePosition));
            }
        }

        /// <summary>
        /// 絕對座標
        /// </summary>
        public Dictionary<string, double> AbsolutePosition
        {
            get => _absolutePosition;
            set
            {
                _absolutePosition = value;

                OnPropertyChanged(nameof(AbsolutePosition));
            }
        }

        /// <summary>
        /// 相對座標
        /// </summary>
        public Dictionary<string, double> RelativePosition
        {
            get => _relativePosition;
            set
            {
                _relativePosition = value;

                OnPropertyChanged(nameof(RelativePosition));
            }
        }

        /// <summary>
        /// 剩餘距離
        /// </summary>
        public Dictionary<string, double> DistanceToGo
        {
            get => _distanceToGo;
            set
            {
                _distanceToGo = value;

                OnPropertyChanged(nameof(DistanceToGo));
            }
        }

        /// <summary>
        /// 設備運轉時間軸紀錄列表
        /// </summary>
        public List<DeviceRuntimeRecord> RuntimeRecords
        {
            get => _runtimeRecords;
            set
            {
                _runtimeRecords = value;

                OnPropertyChanged(nameof(RuntimeRecords));
            }
        }

        /// <summary>
        /// 離線所佔的時間比例
        /// </summary>
        public double OfflineUtilization
        {
            get => _offlineUtilization;
            set
            {
                if (_offlineUtilization == value)
                    return;

                _offlineUtilization = value;

                OnPropertyChanged(nameof(OfflineUtilization));
            }
        }

        /// <summary>
        /// 待機狀態所佔的時間比例
        /// </summary>
        public double StandbyUtilization
        {
            get => _standbyUtilization;
            set
            {
                if (_standbyUtilization == value)
                    return;

                _standbyUtilization = value;

                OnPropertyChanged(nameof(StandbyUtilization));
            }
        }

        /// <summary>
        /// 運轉中所佔的時間比例
        /// </summary>
        public double RunningUtilization
        {
            get => _runningUtilization;
            set
            {
                if (_runningUtilization == value)
                    return;

                _runningUtilization = value;

                OnPropertyChanged(nameof(RunningUtilization));
            }
        }

        /// <summary>
        /// 警報所佔的時間比例
        /// </summary>
        public double AlarmUtilization
        {
            get => _alarmUtilization;
            set
            {
                if (_alarmUtilization == value)
                    return;

                _alarmUtilization = value;

                OnPropertyChanged(nameof(AlarmUtilization));
            }
        }

        /// <summary>
        /// 顯示設備詳細資訊
        /// </summary>
        protected override void ShowDevice()
        {
            _showDetailAction(this);
        }

        /// <summary>
        /// 刪除設備與停止監控，並取消快照訂閱
        /// </summary>
        /// <returns></returns>
        protected override async Task DeleteDevice()
        {
            await base.DeleteDevice();

            // 取消快照訂閱
            _subscriptionDisposable?.Dispose();
        }
    }
}
