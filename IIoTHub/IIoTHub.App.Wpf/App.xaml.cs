using Hardcodet.Wpf.TaskbarNotification;
using IIoTHub.App.Wpf.DependencyInjections;
using IIoTHub.App.Wpf.Services;
using IIoTHub.Application.DependencyInjections;
using IIoTHub.Infrastructure.DependencyInjections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using System.Windows.Media;

namespace IIoTHub.App.Wpf
{
    public partial class App : System.Windows.Application
    {
        private IHost _host;
        private TaskbarIcon _taskbarIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            // 檢查是否已有應用程式實例
            if (!AppSingleInstanceManager.CanCreateInstance())
            {
                // 已有實例，通知現有實例顯示主視窗後退出
                AppSingleInstanceManager.NotifyExistingInstanceToShowWindow();
                this.Shutdown();
                return;
            }
            else
            {
                base.OnStartup(e);

                // 啟動監聽來自其他實例的「顯示主視窗」訊息
                AppSingleInstanceManager.StartListeningForShowWindowMessage(ShowMainWindow);

                // 初始化 HostBuilder
                _host = Host.CreateDefaultBuilder()
                            .ConfigureServices((context, services) =>
                            {
                                services.AddWpf();
                                services.AddApplication();
                                services.AddInfrastructure();
                            })
                            .Build();

                // 啟動 Host
                _host.Start();

                // 取得並初始化托盤圖示
                _taskbarIcon = (TaskbarIcon)FindResource("Taskbar");

                // 顯示主視窗
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
        }

        private void ShowMainWindow()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Show();
                    mainWindow.Activate();
                    mainWindow.WindowState = WindowState.Maximized;
                }
            });
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            _taskbarIcon?.Dispose();

            await _host?.StopAsync();
            _host?.Dispose();

            AppSingleInstanceManager.Dispose();

            base.OnExit(e);
        }

        #region App.xml 設定檔共用屬性

        public static readonly Brush DeviceStatusOfflineBrush =
           (SolidColorBrush)System.Windows.Application.Current.Resources["DeviceStatusOfflineBrush"];

        public static readonly Brush DeviceStatusStandbyBrush =
          (SolidColorBrush)System.Windows.Application.Current.Resources["DeviceStatusStandbyBrush"];

        public static readonly Brush DeviceStatusRunningBrush =
          (SolidColorBrush)System.Windows.Application.Current.Resources["DeviceStatusRunningBrush"];

        public static readonly Brush DeviceStatusAlarmBrush =
          (SolidColorBrush)System.Windows.Application.Current.Resources["DeviceStatusAlarmBrush"];

        #endregion
    }
}
