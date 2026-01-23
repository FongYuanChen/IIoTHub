using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Models.DeviceSettings;
using IIoTHub.Domain.Models.DeviceSnapshots;

namespace IIoTHub.App.Wpf.ViewModels.Dashboard
{
    /// <summary>
    /// 料倉設備的 ViewModel
    /// </summary>
    public class DeviceMagazineViewModel : DeviceViewModelBase
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

        private DeviceMagazineViewModel(IDialogService dialogService,
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
        /// 建立 DeviceMagazineViewModel
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
        public static async Task<DeviceMagazineViewModel> CreateAsync(IDialogService dialogService,
                                                                      IDeviceSettingService deviceService,
                                                                      IDeviceSnapshotMonitorService deviceSnapshotMonitorService,
                                                                      DeviceSetting deviceSetting,
                                                                      IDeviceRuntimeStatisticsService deviceRuntimeStatisticsService,
                                                                      IDeviceSnapshotPublisher deviceSnapshotPublisher,
                                                                      Action snapshotUpdatedAction,
                                                                      Action<DeviceViewModelBase> showDetailAction)
        {
            var deviceViewModel = new DeviceMagazineViewModel(dialogService,
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
                Utilization = e.DeviceRuntimeSummary.RunningUtilization;
            };

            // 訂閱設備快照更新
            _subscriptionDisposable = _deviceSnapshotPublisher.Subscribe<MagazineSnapshot>(_deviceSetting.Id, snapshot =>
            {
                Status = snapshot.RunStatus;
                _snapshotUpdatedAction();
            });
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
