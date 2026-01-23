using System.Windows.Data;

namespace IIoTHub.App.Wpf.ViewModels.MultiValueConverter
{
    /// <summary>
    /// 將設備運行時間長度換算成對應的顯示寬度（像素）
    /// </summary>
    public class RuntimeDurationHourToBarWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2) return 0;
            if (values[0] is TimeSpan duration && values[1] is double totalWidth)
            {
                double hoursInDay = 24;
                var durationHours = duration.TotalHours;
                return durationHours / hoursInDay * totalWidth;
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
