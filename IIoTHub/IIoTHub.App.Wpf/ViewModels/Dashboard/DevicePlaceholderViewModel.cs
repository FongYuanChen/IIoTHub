using IIoTHub.App.Wpf.Interfaces;
using System.Windows.Input;

namespace IIoTHub.App.Wpf.ViewModels.Dashboard
{
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
