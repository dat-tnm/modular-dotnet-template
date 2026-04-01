using System.Globalization;
using System.Windows.Data;
using CompanyName.ProjectName.Hosts.WPF.Framework.Configs;

namespace CompanyName.ProjectName.Hosts.WPF.Converters;

public class ActionButtonStyleConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ActionButtonStyle style)
        {
            return style switch
            {
                ActionButtonStyle.Primary => System.Windows.Application.Current.TryFindResource("PrimaryButtonStyle"),
                ActionButtonStyle.Secondary => System.Windows.Application.Current.TryFindResource("SecondaryButtonStyle"),
                ActionButtonStyle.Danger => System.Windows.Application.Current.TryFindResource("DangerButtonStyle"),
                _ => System.Windows.Application.Current.TryFindResource("PrimaryButtonStyle")
            };
        }
        return System.Windows.Application.Current.TryFindResource("PrimaryButtonStyle");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
