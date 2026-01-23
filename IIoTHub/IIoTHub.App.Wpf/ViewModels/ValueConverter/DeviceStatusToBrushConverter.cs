using IIoTHub.Domain.Enums;
using System.Globalization;
using System.Windows.Data;

namespace IIoTHub.App.Wpf.ViewModels.ValueConverter
{
    /// <summary>
    /// 將設備運行狀態轉換為對應的顏色
    /// </summary>
    public class DeviceStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DeviceRunStatus status)
                return App.DeviceStatusOfflineBrush;

            return status switch
            {
                DeviceRunStatus.Standby => App.DeviceStatusStandbyBrush,
                DeviceRunStatus.Running => App.DeviceStatusRunningBrush,
                DeviceRunStatus.Alarm => App.DeviceStatusAlarmBrush,
                _ => App.DeviceStatusOfflineBrush
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
