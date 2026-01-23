using IIoTHub.App.Wpf.Contexts;
using IIoTHub.Application.Interfaces;
using System.Collections.ObjectModel;

namespace IIoTHub.App.Wpf.ViewModels.DeviceSettingWizardDialog
{
    /// <summary>
    /// 設備設定精靈第二頁的 ViewModel
    /// </summary>
    public class DeviceSettingWizardPage2ViewModel : ViewModelBase
    {
        public DeviceSettingWizardPage2ViewModel(IDeviceDriverMetadataProvider deviceDriverMetadataProvider,
                                                 DeviceSettingWizardDialogContext context)
        {
            context.ParameterSettings
                ??= deviceDriverMetadataProvider.GetDriverMetadata(context.DeviceCategoryType)
                                                .FirstOrDefault(info => info.Name == context.DeviceDriver)?.ParameterSettings
                                                .Select(setting => new DeviceDriverParameterSettingState(setting))
                                                .ToList();

            DriverDisplayName = context.DeviceDriver;
            DriverParameterSettings
                = new ObservableCollection<DeviceDriverParameterSettingViewModel>(
                    context.ParameterSettings.Select(setting => new DeviceDriverParameterSettingViewModel(setting)));
        }

        /// <summary>
        /// 驅動器顯示名稱
        /// </summary>
        public string DriverDisplayName { get; }

        /// <summary>
        /// 設備連線設定列表
        /// </summary>
        public ObservableCollection<DeviceDriverParameterSettingViewModel> DriverParameterSettings { get; }
    }

    /// <summary>
    /// 設備連線設定的 ViewModel
    /// </summary>
    public class DeviceDriverParameterSettingViewModel : ViewModelBase
    {
        private readonly DeviceDriverParameterSettingState _parameterSetting;

        public DeviceDriverParameterSettingViewModel(DeviceDriverParameterSettingState parameterSetting)
        {
            _parameterSetting = parameterSetting;
        }

        /// <summary>
        /// 設定名稱
        /// </summary>
        public string DisplayName => _parameterSetting.DisplayName;

        /// <summary>
        /// 設定說明
        /// </summary>
        public string Note => _parameterSetting.Note;

        /// <summary>
        /// 設定值
        /// </summary>
        public string Value
        {
            get => _parameterSetting.Value;
            set
            {
                if (_parameterSetting.Value == value)
                    return;
                _parameterSetting.Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}
