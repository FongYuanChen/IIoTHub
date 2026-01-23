using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IIoTHub.App.Wpf.ViewModels.ValueConverter
{
    /// <summary>
    /// 將布林值轉換為摺疊面板的左側寬度
    /// </summary>
    public class CollapsePanelLeftWidthConverter : IValueConverter
    {
        public GridLength Expanded { get; set; }
        public GridLength Collapsed { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is bool b && b) ? Expanded : Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
