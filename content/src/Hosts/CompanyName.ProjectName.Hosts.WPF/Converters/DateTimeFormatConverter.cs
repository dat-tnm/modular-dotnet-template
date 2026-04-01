using System.Globalization;
using System.Windows.Data;

namespace CompanyName.ProjectName.Hosts.WPF.Converters;

public class DateTimeFormatConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str && DateTime.TryParse(str, out var dateTime))
        {
            return dateTime;
        }
        return DateTime.MinValue;
    }
}
