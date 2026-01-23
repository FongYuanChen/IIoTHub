using IIoTHub.App.Wpf.Contexts;
using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.App.Wpf.Services;
using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Enums;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace IIoTHub.App.Wpf.ViewModels.DeviceSettingWizardDialog
{
    /// <summary>
    /// 設備設定精靈第一頁的 ViewModel
    /// </summary>
    public class DeviceSettingWizardPage1ViewModel : ViewModelBase
    {
        private readonly IDeviceDriverMetadataProvider _deviceDriverMetadataProvider;
        private readonly DeviceSettingWizardDialogContext _context;
        private readonly BitmapSource _defaultDeviceImageSource =
            new BitmapImage(new Uri("pack://application:,,,/IIoTHub.App.Wpf;component/Resources/DeviceSettingWizardPage1View/DefaultImage.png"));

        public DeviceSettingWizardPage1ViewModel(IImagePickerService imagePickerService,
                                                 IDeviceDriverMetadataProvider deviceDriverMetadataProvider,
                                                 DeviceSettingWizardDialogContext context)
        {
            _deviceDriverMetadataProvider = deviceDriverMetadataProvider;
            _context = context;
            DeviceCategoryTypes.Add(new DeviceCategoryTypeViewModel(DeviceCategoryType.Machine, (vm) => SelectedDeviceCategory = vm));
            DeviceCategoryTypes.Add(new DeviceCategoryTypeViewModel(DeviceCategoryType.Magazine, (vm) => SelectedDeviceCategory = vm));
            DeviceCategoryTypes.Add(new DeviceCategoryTypeViewModel(DeviceCategoryType.Robot, (vm) => SelectedDeviceCategory = vm));
            ChangeDeviceImageCommand = new RelayCommand(async _ =>
            {
                var imagePath = await imagePickerService.PickImageAsync();
                if (!string.IsNullOrWhiteSpace(imagePath))
                {
                    DeviceImageSource = new BitmapImage(new Uri(imagePath));
                }
            });
            Load();
        }

        /// <summary>
        /// 載入初始資料
        /// </summary>
        private void Load()
        {
            var deviceCategoryType = DeviceCategoryTypes.FirstOrDefault(categoryType => _context.DeviceCategoryType == categoryType.Type);
            deviceCategoryType ??= DeviceCategoryTypes.FirstOrDefault();
            if (deviceCategoryType != null)
            {
                deviceCategoryType.IsSelected = true;
            }

            DeviceImageSource = !string.IsNullOrWhiteSpace(_context.DeviceImageBase64String)
                ? ImageHelper.Base64ToBitmapSource(_context.DeviceImageBase64String)
                : _defaultDeviceImageSource;

            var driverMetadata = _deviceDriverMetadataProvider.GetDriverMetadata(deviceCategoryType.Type);
            var drivers = driverMetadata.Select(x => x.Name);
            DeviceDrivers.Clear();
            foreach (var driver in drivers)
            {
                DeviceDrivers.Add(driver);
            }

            SelectedDeviceDriver = DeviceDrivers.FirstOrDefault(driver => driver == _context.DeviceDriver);
            SelectedDeviceDriver ??= DeviceDrivers.FirstOrDefault();
        }

        /// <summary>
        /// 可選設備類型列表
        /// </summary>
        public ObservableCollection<DeviceCategoryTypeViewModel> DeviceCategoryTypes { get; } = [];

        /// <summary>
        /// 當前選擇的設備類型
        /// </summary>
        public DeviceCategoryTypeViewModel SelectedDeviceCategory
        {
            get => DeviceCategoryTypes.FirstOrDefault(e => e.Type == _context.DeviceCategoryType);
            set
            {
                if (_context.DeviceCategoryType != value.Type)
                {
                    _context.DeviceCategoryType = value.Type;
                    _context.ResetDeviceDriverSettings();
                    OnPropertyChanged(nameof(SelectedDeviceCategory));

                    Load();
                }
            }
        }

        /// <summary>
        /// 變更設備圖片命令
        /// </summary>
        public ICommand ChangeDeviceImageCommand { get; }

        /// <summary>
        /// 設備 ID
        /// </summary>
        public string DeviceId => _context.DeviceId.ToString();

        /// <summary>
        /// 設備名稱
        /// </summary>
        public string DeviceName
        {
            get => _context.DeviceName;
            set
            {
                if (_context.DeviceName != value)
                {
                    _context.DeviceName = value;
                    OnPropertyChanged(nameof(DeviceName));
                }
            }
        }

        /// <summary>
        /// 設備圖片來源
        /// </summary>
        public BitmapSource DeviceImageSource
        {
            get => ImageHelper.Base64ToBitmapSource(_context.DeviceImageBase64String);
            set
            {
                var imageBase64String = ImageHelper.BitmapSourceToBase64(value);
                if (_context.DeviceImageBase64String != imageBase64String)
                {
                    _context.DeviceImageBase64String = imageBase64String;
                    OnPropertyChanged(nameof(DeviceImageSource));
                }
            }
        }

        /// <summary>
        /// 可選設備驅動列表
        /// </summary>
        public ObservableCollection<string> DeviceDrivers { get; } = [];

        /// <summary>
        /// 當前選擇的設備驅動
        /// </summary>
        public string SelectedDeviceDriver
        {
            get => _context.DeviceDriver;
            set
            {
                if (_context.DeviceDriver != value)
                {
                    _context.DeviceDriver = value;
                    _context.ResetDeviceDriverSettings();
                    OnPropertyChanged(nameof(SelectedDeviceDriver));
                }
            }
        }
    }

    /// <summary>
    /// 設備類型 ViewModel
    /// </summary>
    public class DeviceCategoryTypeViewModel : ViewModelBase
    {
        private bool _isSelected;
        private readonly DeviceCategoryType _type;
        private readonly string _description;
        private readonly string _iconPath;
        private readonly Action<DeviceCategoryTypeViewModel> _selectedAction;

        public DeviceCategoryTypeViewModel(DeviceCategoryType type,
                                           Action<DeviceCategoryTypeViewModel> selectedAction)
        {
            _type = type;
            _description = _type switch
            {
                DeviceCategoryType.Machine => "加工機",
                DeviceCategoryType.Magazine => "料倉",
                DeviceCategoryType.Robot => "機械手臂",
                _ => _type.ToString()
            };
            _iconPath = _type switch
            {
                DeviceCategoryType.Machine => "pack://application:,,,/IIoTHub.App.Wpf;component/Resources/DeviceSettingWizardPage1View/MachineIcon.png",
                DeviceCategoryType.Magazine => "pack://application:,,,/IIoTHub.App.Wpf;component/Resources/DeviceSettingWizardPage1View/MagazineIcon.png",
                DeviceCategoryType.Robot => "pack://application:,,,/IIoTHub.App.Wpf;component/Resources/DeviceSettingWizardPage1View/RobotIcon.png",
                _ => string.Empty
            };
            _selectedAction = selectedAction;
        }

        /// <summary>
        /// 設備類型
        /// </summary>
        public DeviceCategoryType Type => _type;

        /// <summary>
        /// 類型描述文字
        /// </summary>
        public string Description => _description;

        /// <summary>
        /// 類型圖示路徑
        /// </summary>
        public string IconPath => _iconPath;

        /// <summary>
        /// 是否被選擇
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));

                if (_isSelected)
                {
                    _selectedAction?.Invoke(this);
                }
            }
        }
    }
}
