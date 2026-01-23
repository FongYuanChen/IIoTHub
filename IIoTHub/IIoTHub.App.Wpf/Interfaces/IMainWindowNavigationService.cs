using IIoTHub.App.Wpf.ViewModels;

namespace IIoTHub.App.Wpf.Services
{
    /// <summary>
    /// 主視窗導覽服務介面
    /// </summary>
    public interface IMainWindowNavigationService
    {
        /// <summary>
        /// 當前的ViewModel
        /// </summary>
        ViewModelBase CurrentViewModel { get; }

        /// <summary>
        /// 導覽到 Dashboard 視圖
        /// </summary>
        Task NavigateToDashboardAsync();
    }
}
