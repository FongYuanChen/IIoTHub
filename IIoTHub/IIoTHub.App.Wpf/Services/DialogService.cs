using IIoTHub.App.Wpf.Contexts;
using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.App.Wpf.ViewModels.DeviceSettingWizardDialog;
using IIoTHub.App.Wpf.Views.DeviceSettingWizardDialog;
using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Models.DeviceSettings;
using System.Windows;

namespace IIoTHub.App.Wpf.Services
{
    public class DialogService : IDialogService
    {
        private readonly IDeviceDriverMetadataProvider _deviceDriverMetadataProvider;
        private readonly IDeviceSettingService _deviceSettingService;
        private readonly IImagePickerService _imagePickerService;

        public DialogService(IDeviceDriverMetadataProvider deviceDriverMetadataProvider,
                             IDeviceSettingService deviceSettingService,
                             IImagePickerService imagePickerService)
        {
            _deviceDriverMetadataProvider = deviceDriverMetadataProvider;
            _deviceSettingService = deviceSettingService;
            _imagePickerService = imagePickerService;
        }

        /// <summary>
        /// 顯示設備設定精靈對話框
        /// </summary>
        /// <param name="deviceSetting"></param>
        public void ShowDeviceSettingWizardDialog(DeviceSetting deviceSetting = null)
        {
            // 建立對話框上下文
            var context = deviceSetting == null
                ? DeviceSettingWizardDialogContext.CreateNew()
                : DeviceSettingWizardDialogContext.FromDeviceSetting(deviceSetting);

            // 建立精靈對話框的 ViewModel，包含多頁導覽
            var viewModel = new DeviceSettingWizardDialogViewModel(
                _deviceSettingService,
                new WizardDialogNavigation([
                    () => new DeviceSettingWizardPage1ViewModel(_imagePickerService, _deviceDriverMetadataProvider, context),
                    () => new DeviceSettingWizardPage2ViewModel(_deviceDriverMetadataProvider, context)
                ]),
                context);

            // 建立對話框視圖並顯示
            var dialog = new DeviceSettingWizardDialogView(viewModel);
            ShowDialog(dialog);
        }

        /// <summary>
        /// 顯示指定對話框視窗
        /// </summary>
        /// <param name="dialog"></param>
        private static void ShowDialog(Window dialog)
        {
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            dialog.ShowDialog();
        }

        /// <summary>
        /// 顯示確認對話框，回傳使用者是否點擊「是」
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool ShowConfirmationDialog(string message, string title = "確認")
        {
            var result = MessageBox.Show(
                message,
                title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            return result == MessageBoxResult.Yes;
        }
    }
}
