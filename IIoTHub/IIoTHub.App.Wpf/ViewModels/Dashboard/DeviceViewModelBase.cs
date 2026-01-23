using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.App.Wpf.Services;
using IIoTHub.Application.Enums;
using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Models.DeviceSettings;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace IIoTHub.App.Wpf.ViewModels.Dashboard
{
    /// <summary>
    /// 設備的 ViewModel
    /// </summary>
    public abstract class DeviceViewModelBase : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IDeviceSettingService _deviceService;
        private readonly IDeviceSnapshotMonitorService _deviceSnapshotMonitorService;

        private DeviceSetting _deviceSetting;

        private bool _isMonitoring = false;
        private DeviceRunStatus _status = DeviceRunStatus.Offline;
        private double _utilization = 0;

        public DeviceViewModelBase(IDialogService dialogService,
                                   IDeviceSettingService deviceService,
                                   IDeviceSnapshotMonitorService deviceSnapshotMonitorService,
                                   DeviceSetting deviceSetting)
        {
            _dialogService = dialogService;
            _deviceService = deviceService;
            _deviceSnapshotMonitorService = deviceSnapshotMonitorService;

            _deviceSetting = deviceSetting;

            MonitorCommand = new RelayCommand(async _ => await MonitorDevice());
            ViewCommand = new RelayCommand(_ => ShowDevice());
            EditCommand = new RelayCommand(_ => EditDevice());
            CopyCommand = new RelayCommand(async _ => await CopyDevice());
            DeleteCommand = new RelayCommand(async _ => await DeleteDevice());
        }

        /// <summary>
        /// 對應的設備設定資料
        /// </summary>
        public DeviceSetting DeviceSetting
        {
            get => _deviceSetting;
            private set
            {
                if (_deviceSetting == value)
                    return;

                _deviceSetting = value;
                OnPropertyChanged(nameof(DeviceSetting));
                RefreshDeviceSettingDerivedProperties();
            }
        }

        /// <summary>
        /// 更新設備設定資料
        /// </summary>
        /// <param name="newSetting"></param>
        public void UpdateDeviceSetting(DeviceSetting newSetting)
        {
            DeviceSetting = newSetting;
        }

        /// <summary>
        /// 刷新所有依賴 DeviceSetting 的衍生屬性
        /// </summary>
        private void RefreshDeviceSettingDerivedProperties()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ImageSource));
        }

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
        protected virtual async Task InitializeAsync()
        {
            // 同步監控狀態
            IsMonitoring = await _deviceSnapshotMonitorService.IsMonitoringAsync(DeviceSetting.Id);

            // 若本來已監控，則啟動監控
            if (IsMonitoring)
            {
                await _deviceSnapshotMonitorService.StartMonitorAsync(DeviceSetting.Id);
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
                await _deviceSnapshotMonitorService.StartMonitorAsync(DeviceSetting.Id);
            }
            else
            {
                await _deviceSnapshotMonitorService.StopMonitorAsync(DeviceSetting.Id, StopMonitorReason.Temporary);
            }
        }

        /// <summary>
        /// 顯示設備詳細資訊
        /// </summary>
        protected virtual void ShowDevice()
        {
            
        }

        /// <summary>
        /// 編輯設備設定
        /// </summary>
        protected virtual void EditDevice()
        {
            _dialogService.ShowDeviceSettingWizardDialog(DeviceSetting);
        }

        /// <summary>
        /// 複製設備設定
        /// </summary>
        /// <returns></returns>
        protected virtual async Task CopyDevice()
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
        protected virtual async Task DeleteDevice()
        {
            // 顯示確認對話框
            bool confirm = _dialogService.ShowConfirmationDialog(
                $"是否確定要刪除設備「{DeviceSetting.Name}」？",
                "刪除設備");

            if (!confirm)
                return; // 使用者取消刪除

            // 停止監控
            IsMonitoring = false;
            await _deviceSnapshotMonitorService.StopMonitorAsync(DeviceSetting.Id, StopMonitorReason.Removed);

            // 刪除設備設定
            await _deviceService.DeleteAsync(DeviceSetting.Id);
        }
    }
}
