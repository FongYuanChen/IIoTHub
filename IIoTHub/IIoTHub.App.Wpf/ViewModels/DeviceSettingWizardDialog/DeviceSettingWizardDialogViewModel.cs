using IIoTHub.App.Wpf.Contexts;
using IIoTHub.App.Wpf.Services;
using IIoTHub.Application.Interfaces;
using System.Windows;
using System.Windows.Input;

namespace IIoTHub.App.Wpf.ViewModels.DeviceSettingWizardDialog
{
    /// <summary>
    /// 設備設定精靈對話框的 ViewModel
    /// </summary>
    public class DeviceSettingWizardDialogViewModel : ViewModelBase
    {
        private readonly IDeviceSettingService _deviceSettingService;
        private readonly WizardDialogNavigation _wizardDialogNavigation;
        private readonly DeviceSettingWizardDialogContext _context;

        public DeviceSettingWizardDialogViewModel(IDeviceSettingService deviceSettingService,
                                                  WizardDialogNavigation wizardDialogNavigation,
                                                  DeviceSettingWizardDialogContext context)
        {
            _deviceSettingService = deviceSettingService;
            _wizardDialogNavigation = wizardDialogNavigation;
            _context = context;

            PreviousCommand = new RelayCommand(_ => MovePrevious(), _ => _wizardDialogNavigation.CanMovePrevious);
            NextCommand = new RelayCommand(_ => MoveNext(), _ => _wizardDialogNavigation.CanMoveNext);
            FinishCommand = new RelayCommand(async parameter =>
            {
                await MoveFinish();

                if (parameter is Window window)
                {
                    window.Close();
                }
            });
        }

        /// <summary>
        /// 返回上一頁命令
        /// </summary>
        public ICommand PreviousCommand { get; }

        /// <summary>
        /// 移動到下一頁命令
        /// </summary>
        public ICommand NextCommand { get; }

        /// <summary>
        /// 完成命令，新增或更新設備設定
        /// </summary>
        public ICommand FinishCommand { get; }

        /// <summary>
        /// 目前的ViewModel
        /// </summary>
        public object CurrentPageViewModel =>
            _wizardDialogNavigation.CurrentPageViewModel;

        // <summary>
        /// 上一頁按鈕的可見性
        /// </summary>
        public Visibility PreviousButtonVisibility =>
            _wizardDialogNavigation.CanMovePrevious ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// 下一頁按鈕的可見性
        /// </summary>
        public Visibility NextButtonVisibility =>
            _wizardDialogNavigation.CanMoveNext ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// 完成按鈕的可見性
        /// </summary>
        public Visibility FinishButtonVisibility =>
            !_wizardDialogNavigation.CanMoveNext ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// 移動到上一頁
        /// </summary>
        private void MovePrevious()
        {
            _wizardDialogNavigation.MovePrevious();
            RefreshNavigationState();
        }

        /// <summary>
        /// 移動到下一頁
        /// </summary>
        private void MoveNext()
        {
            _wizardDialogNavigation.MoveNext();
            RefreshNavigationState();
        }

        /// <summary>
        /// 完成，將上下文資料轉換為 DeviceSetting 並儲存
        /// </summary>
        /// <returns></returns>
        private async Task MoveFinish()
        {
            var deviceSetting = await _deviceSettingService.GetByIdAsync(_context.DeviceId);
            if (deviceSetting != null)
            {
                await _deviceSettingService.UpdateAsync(_context.ConvertToDeviceSetting());
            }
            else
            {
                await _deviceSettingService.AddAsync(_context.ConvertToDeviceSetting());
            }
        }

        /// <summary>
        /// 更新頁面與按鈕的狀態
        /// </summary>
        private void RefreshNavigationState()
        {
            OnPropertyChanged(nameof(CurrentPageViewModel));
            OnPropertyChanged(nameof(PreviousButtonVisibility));
            OnPropertyChanged(nameof(NextButtonVisibility));
            OnPropertyChanged(nameof(FinishButtonVisibility));
        }
    }
}
