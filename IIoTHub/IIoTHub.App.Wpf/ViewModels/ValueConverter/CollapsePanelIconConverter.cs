using System.Globalization;
using System.Windows.Data;

namespace IIoTHub.App.Wpf.ViewModels.ValueConverter
{
    /// <summary>
    /// 將布林值轉換為摺疊面板的摺疊/展開圖示
    /// </summary>
    public class CollapsePanelIconConverter : IValueConverter
    {
        public string ExpandedIcon { get; set; }
        public string CollapsedIcon { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is bool b && b) ? ExpandedIcon : CollapsedIcon;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
