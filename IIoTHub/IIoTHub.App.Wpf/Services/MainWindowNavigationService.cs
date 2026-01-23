using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.App.Wpf.ViewModels;
using IIoTHub.App.Wpf.ViewModels.Dashboard;
using IIoTHub.Application.Interfaces;

namespace IIoTHub.App.Wpf.Services
{
    public class MainWindowNavigationService : IMainWindowNavigationService
    {
        private readonly IDialogService _dialogService;
        private readonly IDeviceSettingService _deviceSettingService;
        private readonly IDeviceSnapshotMonitorService _deviceSnapshotMonitorService;
        private readonly IDeviceSnapshotPublisher _deviceSnapshotPublisher;
        private readonly IDeviceRuntimeStatisticsService _deviceRuntimeStatisticsService;

        private ViewModelBase _currentViewModel;

        public MainWindowNavigationService(IDialogService dialogService,
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
        }

        /// <summary>
        /// 目前主視窗顯示的內容
        /// </summary>
        public ViewModelBase CurrentViewModel => _currentViewModel;

        /// <summary>
        /// 導覽到 Dashboard 視圖
        /// </summary>
        public async Task NavigateToDashboardAsync()
        {
            _currentViewModel = await DashboardViewModel.CreateAsync(
                _dialogService,
                _deviceSettingService,
                _deviceSnapshotMonitorService,
                _deviceSnapshotPublisher,
                _deviceRuntimeStatisticsService);
        }
    }
}