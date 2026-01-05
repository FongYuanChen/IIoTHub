using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.App.Wpf.Services;
using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Models.DeviceSettings;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace IIoTHub.App.Wpf.ViewModels.MainWindow
{
    /// <summary>
    /// 儀表板的 ViewModel
    /// </summary>
    public class DashboardViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IDeviceSettingService _deviceSettingService;
        private readonly IDeviceMonitorService _deviceMonitorService;

        public DashboardViewModel(IDialogService dialogService,
                                  IDeviceSettingService deviceSettingService,
                                  IDeviceMonitorService deviceMonitorService)
        {
            _dialogService = dialogService;
            _deviceSettingService = deviceSettingService;
            _deviceMonitorService = deviceMonitorService;
            _deviceSettingService.DeviceSettingChanged += async (s, e) => { await LoadAsync(); };
            _ = LoadAsync();
        }

        /// <summary>
        /// 監控的設備列表
        /// </summary>
        public ObservableCollection<DeviceViewModel> Devices { get; set; } = [];

        /// <summary>
        /// 包含新增按鈕的設備列表，用於 ItemsControl 顯示
        /// </summary>
        public ObservableCollection<object> DevicesWithAddButton { get; } = [];

        /// <summary>
        /// 平均稼動率，只計算正在監控的設備
        /// </summary>
        public double AverageUtilization
            => Devices.Where(device => device.IsMonitoring)
                      .Select(device => device.Utilization)
                      .DefaultIfEmpty(0)
                      .Average();

        /// <summary>
        /// 設備數量
        /// </summary>
        public int DeviceCount => Devices.Count;

        /// <summary>
        /// 待機設備數量
        /// </summary>
        public int StandbyDeviceCount
            => Devices.Count(device => device.Status == DeviceRunStatus.Standby);

        /// <summary>
        /// 運轉設備數量
        /// </summary>
        public int RunDeviceCount
            => Devices.Count(device => device.Status == DeviceRunStatus.Running);

        /// <summary>
        /// 警報設備數量
        /// </summary>
        public int AlarmDeviceCount
            => Devices.Count(device => device.Status == DeviceRunStatus.Alarm);

        /// <summary>
        /// 離線設備數量
        /// </summary>
        public int OfflineDeviceCount
            => Devices.Count(device => device.Status == DeviceRunStatus.Offline);

        /// <summary>
        /// 載入設備列表與初始化監控卡片
        /// </summary>
        /// <returns></returns>
        private async Task LoadAsync()
        {
            var deviceSettings = await _deviceSettingService.GetAllAsync();
            var devices = deviceSettings.Select(setting
                => new DeviceViewModel(_dialogService,
                                       _deviceSettingService,
                                       _deviceMonitorService,
                                       setting,
                                       () => RefreshSummary()));
            Devices.Clear();
            foreach (var device in devices)
            {
                Devices.Add(device);
            }

            DevicesWithAddButton.Clear();
            foreach (var device in Devices)
            {
                DevicesWithAddButton.Add(device);
            }
            DevicesWithAddButton.Add(new DevicePlaceholderViewModel(_dialogService));

            RefreshSummary();
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
    }

    /// <summary>
    /// 設備的 ViewModel
    /// </summary>
    public class DeviceViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IDeviceSettingService _deviceService;
        private readonly IDeviceMonitorService _deviceMonitorService;
        private readonly DeviceSetting _deviceSetting;
        private readonly Action _snapshotUpdatedAction;

        private bool _isMonitoring = false;
        private DeviceRunStatus _status = DeviceRunStatus.Offline;
        private double _utilization = 0;

        public DeviceViewModel(IDialogService dialogService,
                               IDeviceSettingService deviceService,
                               IDeviceMonitorService deviceMonitorService,
                               DeviceSetting deviceSetting,
                               Action snapshotUpdatedAction)
        {
            _dialogService = dialogService;
            _deviceService = deviceService;
            _deviceMonitorService = deviceMonitorService;
            _deviceSetting = deviceSetting;
            _snapshotUpdatedAction = snapshotUpdatedAction;

            MonitorCommand = new RelayCommand(async _ => await MonitorDevice());
            ViewCommand = new RelayCommand(_ => ShowDevice());
            EditCommand = new RelayCommand(_ => EditDevice());
            CopyCommand = new RelayCommand(async _ => await CopyDevice());
            DeleteCommand = new RelayCommand(async _ => await DeleteDevice());

            _ = InitializeAsync();
        }

        /// <summary>
        /// 對應的設備設定資料
        /// </summary>
        public DeviceSetting DeviceSetting => _deviceSetting;

        /// <summary>
        /// 設備名稱
        /// </summary>
        public string Name => _deviceSetting.Name;

        /// <summary>
        /// 設備圖示
        /// </summary>
        public BitmapSource ImageSource
            => ImageHelper.Base64ToBitmapSource(_deviceSetting.ImageBase64String);

        /// <summary>
        /// 是否正在監控
        /// </summary>
        public bool IsMonitoring
        {
            get => _isMonitoring;
            set
            {
                if (_isMonitoring == value)
                    return;

                _isMonitoring = value;
                OnPropertyChanged(nameof(IsMonitoring));
            }
        }

        /// <summary>
        /// 設備運行狀態
        /// </summary>
        public DeviceRunStatus Status
        {
            get => _status;
            set
            {
                if (_status == value)
                    return;

                _status = value;

                OnPropertyChanged(nameof(Status));
            }
        }

        /// <summary>
        /// 設備稼動率
        /// </summary>
        public double Utilization
        {
            get => _utilization;
            set
            {
                if (_utilization == value)
                    return;

                _utilization = value;

                OnPropertyChanged(nameof(Utilization));
            }
        }

        /// <summary>
        /// 開關監控命令
        /// </summary>
        public ICommand MonitorCommand { get; }

        /// <summary>
        /// 查看設備詳細資訊命令
        /// </summary>
        public ICommand ViewCommand { get; }

        /// <summary>
        /// 編輯設備命令
        /// </summary>
        public ICommand EditCommand { get; }

        /// <summary>
        /// 複製設備命令
        /// </summary>
        public ICommand CopyCommand { get; }

        /// <summary>
        /// 刪除設備命令
        /// </summary>
        public ICommand DeleteCommand { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            // 添加快照訂閱
            _deviceMonitorService.Subscribe(_deviceSetting.Id, snapshot =>
            {
                Status = snapshot.RunStatus;
                Utilization = snapshot.Utilization;

                _snapshotUpdatedAction();
            });

            // 同步狀態
            IsMonitoring = await _deviceMonitorService.IsMonitoring(DeviceSetting.Id);

            // 若本來就是監控中，明確啟動
            if (IsMonitoring)
            {
                await _deviceMonitorService.StartMonitor(DeviceSetting.Id);
            }
        }

        /// <summary>
        /// 開始或停止監控設備
        /// </summary>
        /// <returns></returns>
        private async Task MonitorDevice()
        {
            if (IsMonitoring)
            {
                await _deviceMonitorService.StartMonitor(DeviceSetting.Id);
            }
            else
            {
                await _deviceMonitorService.StopMonitor(DeviceSetting.Id);


                Status = DeviceRunStatus.Offline;
            }
        }

        /// <summary>
        /// 顯示設備詳細資訊
        /// </summary>
        private void ShowDevice()
        {
            // 顯示對話框
        }

        /// <summary>
        /// 編輯設備設定
        /// </summary>
        private void EditDevice()
        {
            _dialogService.ShowDeviceSettingWizardDialog(DeviceSetting);
        }

        /// <summary>
        /// 複製設備設定
        /// </summary>
        /// <returns></returns>
        private async Task CopyDevice()
        {
            var devices = await _deviceService.GetAllAsync();
            var newId = Guid.NewGuid();
            var newName = GenerateCopyName(DeviceSetting.Name,
                                           devices.Select(device => device.Name));
            await _deviceService.AddAsync(
                new DeviceSetting(newId,
                                  newName,
                                  DeviceSetting.ImageBase64String,
                                  DeviceSetting.CategoryType,
                                  DeviceSetting.DriverSetting));
        }

        /// <summary>
        /// 產生複製名稱
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="existingNames"></param>
        /// <returns></returns>
        private static string GenerateCopyName(string originalName, IEnumerable<string> existingNames)
        {
            int maxIndex = existingNames
                .Select(name =>
                {
                    if (name.StartsWith(originalName + "(") && name.EndsWith(")"))
                    {
                        var inner = name.Substring(originalName.Length + 1, name.Length - originalName.Length - 2);
                        return int.TryParse(inner, out int index) ? index : 0;
                    }
                    else
                    {
                        return 0;
                    }
                })
                .Max();
            return $"{originalName}({maxIndex + 1})";
        }

        /// <summary>
        /// 刪除設備與停止監控
        /// </summary>
        /// <returns></returns>
        private async Task DeleteDevice()
        {
            // 顯示確認對話框
            bool confirm = _dialogService.ShowConfirmationDialog(
                $"是否確定要刪除設備「{DeviceSetting.Name}」？",
                "刪除設備");

            if (!confirm)
                return; // 使用者取消刪除

            IsMonitoring = false;
            await _deviceMonitorService.StopMonitor(DeviceSetting.Id);
            await _deviceService.DeleteAsync(DeviceSetting.Id);
        }
    }

    /// <summary>
    /// 新增設備按鈕的 ViewModel
    /// </summary>
    public class DevicePlaceholderViewModel
    {
        private readonly IDialogService _dialogService;

        public DevicePlaceholderViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            AddCommand = new RelayCommand(_ => AddDevice());
        }

        /// <summary>
        /// 新增設備命令
        /// </summary>
        public ICommand AddCommand { get; }

        /// <summary>
        /// 顯示新增設備對話框
        /// </summary>
        private void AddDevice()
        {
            _dialogService.ShowDeviceSettingWizardDialog();
        }
    }
}
