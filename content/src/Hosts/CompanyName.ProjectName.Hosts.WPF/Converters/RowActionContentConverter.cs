using System.Globalization;
using System.Windows.Data;
using CompanyName.ProjectName.Hosts.WPF.Framework.Configs;

namespace CompanyName.ProjectName.Hosts.WPF.Converters;

/// <summary>
/// Converts RowActionType to display content (icon/text)
/// </summary>
public class RowActionContentConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 3) return string.Empty;
        
        var actionType = values[0] as RowActionType? ?? RowActionType.Custom;
        var customText = values[1] as string;
        var isActive = values[2] as bool?;

        return actionType switch
        {
            RowActionType.Toggle => isActive == true ? "Deactivate" : "Activate",
            RowActionType.Edit => "✎ Edit",
            RowActionType.Delete => "🗑 Delete",
            RowActionType.Custom => customText ?? "Action",
            _ => string.Empty
        };
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
