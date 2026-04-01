using System.Globalization;
using System.Windows.Data;

namespace CompanyName.ProjectName.Hosts.WPF.Converters;

public class TruncateTextConverter : IValueConverter
{
    public int MaxLength { get; set; } = 50;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string text)
        {
            int maxLen = parameter is int paramLen ? paramLen : MaxLength;
            
            if (text.Length <= maxLen)
                return text;
            
            return text[..maxLen] + "...";
        }
        
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
