using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.Application.Enums;
using IIoTHub.Application.Interfaces;
using IIoTHub.Application.Models;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Models.DeviceSettings;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace IIoTHub.App.Wpf.ViewModels.Dashboard
{
    /// <summary>
    /// 儀表板的 ViewModel
    /// </summary>
    public class DashboardViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IDeviceSettingService _deviceSettingService;
        private readonly IDeviceSnapshotMonitorService _deviceSnapshotMonitorService;
        private readonly IDeviceSnapshotPublisher _deviceSnapshotPublisher;
        private readonly IDeviceRuntimeStatisticsService _deviceRuntimeStatisticsService;

        private readonly SemaphoreSlim _deviceSettingChangedLock = new(1, 1);

        private DashboardViewModel(IDialogService dialogService,
                                   IDeviceSettingService deviceSettingService,
                                   IDeviceSnapshotMonitorService deviceSnapshotMonitorService,
                                   IDeviceSnapshotPublisher deviceSnapshotPublisher,
                                   IDeviceRuntimeStatisticsService deviceRuntimeStatisticsService)
        {
            _dialogService = dialogService;
            _deviceSettingService = deviceSettingService;
            _deviceSnapshotMonitorService = deviceSnapshotMonitorService;
            _deviceSnapshotPublisher = deviceSnapshotPublisher;
            _deviceRuntimeStatisticsService = deviceRuntimeStatisticsService;
            _deviceSettingService.DeviceSettingChanged += (s, e) => _ = OnDeviceSettingChangedAsync(s, e);
        }

        /// <summary>
        /// 建立 DashboardViewModel
        /// </summary>
        /// <param name="dialogService"></param>
        /// <param name="deviceSettingService"></param>
        /// <param name="deviceMonitorService"></param>
        /// <returns></returns>
        public static async Task<DashboardViewModel> CreateAsync(IDialogService dialogService,
                                                                 IDeviceSettingService deviceSettingService,
                                                                 IDeviceSnapshotMonitorService deviceSnapshotMonitorService,
                                                                 IDeviceSnapshotPublisher deviceSnapshotPublisher,
                                                                 IDeviceRuntimeStatisticsService deviceRuntimeStatisticsService)
        {
            var dashboardViewModel = new DashboardViewModel(dialogService,
                                                            deviceSettingService,
                                                            deviceSnapshotMonitorService,
                                                            deviceSnapshotPublisher,
                                                            deviceRuntimeStatisticsService);
            await dashboardViewModel.InitializeAsync();
            return dashboardViewModel;
        }

        /// <summary>
        /// 包含新增按鈕的設備列表
        /// </summary>
        public ObservableCollection<object> DevicesWithAddButton { get; } = [];

        /// <summary>
        /// 平均稼動率，只計算正在監控的設備
        /// </summary>
        public double AverageUtilization
            => DevicesWithAddButton.OfType<DeviceViewModelBase>()
                .Where(device => device.IsMonitoring)
                .Select(device => device.Utilization)
                .DefaultIfEmpty(0)
                .Average();

        /// <summary>
        /// 設備數量
        /// </summary>
        public int DeviceCount
            => DevicesWithAddButton.OfType<DeviceViewModelBase>().Count();

        /// <summary>
        /// 待機設備數量
        /// </summary>
        public int StandbyDeviceCount
            => DevicesWithAddButton.OfType<DeviceViewModelBase>()
                .Count(device => device.Status == DeviceRunStatus.Standby);

        /// <summary>
        /// 運轉設備數量
        /// </summary>
        public int RunDeviceCount
            => DevicesWithAddButton.OfType<DeviceViewModelBase>()
                .Count(device => device.Status == DeviceRunStatus.Running);

        /// <summary>
        /// 警報設備數量
        /// </summary>
        public int AlarmDeviceCount
            => DevicesWithAddButton.OfType<DeviceViewModelBase>()
                .Count(device => device.Status == DeviceRunStatus.Alarm);

        /// <summary>
        /// 離線設備數量
        /// </summary>
        public int OfflineDeviceCount
            => DevicesWithAddButton.OfType<DeviceViewModelBase>()
                .Count(device => device.Status == DeviceRunStatus.Offline);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            var deviceSettings = await _deviceSettingService.GetAllAsync();

            var deviceTasks = deviceSettings.Select(async setting => await CreateDeviceViewModel(setting));

            var devices = await Task.WhenAll(deviceTasks);

            DevicesWithAddButton.Clear();
            foreach (var device in devices)
            {
                DevicesWithAddButton.Add(device);
            }
            DevicesWithAddButton.Add(new DevicePlaceholderViewModel(_dialogService));

            RefreshSummary();
        }

        /// <summary>
        /// 處理設備設定變更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task OnDeviceSettingChangedAsync(object sender, DeviceSettingChangedEventArgs e)
        {
            await _deviceSettingChangedLock.WaitAsync();
            try
            {
                var device = DevicesWithAddButton
                    .OfType<DeviceViewModelBase>()
                    .FirstOrDefault(device => device.DeviceSetting.Id == e.DeviceSetting.Id);

                switch (e.ChangeType)
                {
                    case DeviceSettingChangeType.Added:
                        if (device == null)
                        {
                            var index = DevicesWithAddButton.Count - 1;
                            var deviceViewModel = await CreateDeviceViewModel(e.DeviceSetting);
                            DevicesWithAddButton.Insert(index, deviceViewModel);
                        }
                        break;
                    case DeviceSettingChangeType.Updated:
                        device?.UpdateDeviceSetting(e.DeviceSetting);
                        break;
                    case DeviceSettingChangeType.Deleted:
                        if (device != null)
                        {
                            var index = DevicesWithAddButton.IndexOf(device);
                            DevicesWithAddButton.RemoveAt(index);
                        }
                        break;
                }

                RefreshSummary();
            }
            finally
            {
                _deviceSettingChangedLock.Release();
            }
        }

        /// <summary>
        /// 建立設備的 ViewModel
        /// </summary>
        /// <param name="deviceSetting"></param>
        /// <returns></returns>
        private async Task<object> CreateDeviceViewModel(DeviceSetting deviceSetting)
        {
            return deviceSetting.CategoryType switch
            {
                DeviceCategoryType.Machine
                    => await DeviceMachineViewModel.CreateAsync(
                        _dialogService,
                        _deviceSettingService,
                        _deviceSnapshotMonitorService,
                        deviceSetting,
                        _deviceRuntimeStatisticsService,
                        _deviceSnapshotPublisher,
                        RefreshSummary,
                        SetSelectedDeviceViewModel),

                DeviceCategoryType.Magazine
                    => await DeviceMagazineViewModel.CreateAsync(
                        _dialogService,
                        _deviceSettingService,
                        _deviceSnapshotMonitorService,
                        deviceSetting,
                        _deviceRuntimeStatisticsService,
                        _deviceSnapshotPublisher,
                        RefreshSummary,
                        SetSelectedDeviceViewModel),

                DeviceCategoryType.Robot
                    => await DeviceRobotViewModel.CreateAsync(
                        _dialogService,
                        _deviceSettingService,
                        _deviceSnapshotMonitorService,
                        deviceSetting,
                        _deviceRuntimeStatisticsService,
                        _deviceSnapshotPublisher,
                        RefreshSummary,
                        SetSelectedDeviceViewModel)
            };
        }

        /// <summary>
        /// 觸發統計數據屬性變更通知
        /// </summary>
        private void RefreshSummary()
        {
            OnPropertyChanged(nameof(AverageUtilization));
            OnPropertyChanged(nameof(DeviceCount));
            OnPropertyChanged(nameof(StandbyDeviceCount));
            OnPropertyChanged(nameof(RunDeviceCount));
            OnPropertyChanged(nameof(AlarmDeviceCount));
            OnPropertyChanged(nameof(OfflineDeviceCount));
        }


        #region 設備詳細資訊面板

        private DeviceViewModelBase _selectedDeviceViewModel;
        private bool _isDetailPanelExpanded = false;

        /// <summary>
        /// 設定被選取的設備 ViewModel
        /// </summary>
        /// <param name="deviceViewModel"></param>
        private void SetSelectedDeviceViewModel(DeviceViewModelBase deviceViewModel)
        {
            SelectedDeviceViewModel = deviceViewModel;
        }

        /// <summary>
        /// 目前被選取的設備 ViewModel
        /// </summary>
        public DeviceViewModelBase SelectedDeviceViewModel
        {
            get => _selectedDeviceViewModel;
            set
            {
                _selectedDeviceViewModel = value;
                OnPropertyChanged(nameof(SelectedDeviceViewModel));

                // 有選取設備時自動展開詳情面板，否則收合
                IsDeviceDetailPaneExpanded = value != null;
            }
        }

        /// <summary>
        /// 設備詳細資訊面板是否處於展開狀態
        /// </summary>
        public bool IsDeviceDetailPaneExpanded
        {
            get => _isDetailPanelExpanded;
            set
            {
                if (_isDetailPanelExpanded == value)
                    return;

                _isDetailPanelExpanded = value;

                OnPropertyChanged(nameof(IsDeviceDetailPaneExpanded));
            }
        }

        /// <summary>
        /// 切換設備詳細資訊面板顯示狀態的命令
        /// </summary>
        public ICommand ToggleDeviceDetailPaneCommand => new RelayCommand(_ =>
        {
            IsDeviceDetailPaneExpanded = !IsDeviceDetailPaneExpanded;
        });

        #endregion

    }
}
