using System.Globalization;
using System.Windows.Data;
using CompanyName.ProjectName.Hosts.WPF.Framework.Configs;

namespace CompanyName.ProjectName.Hosts.WPF.Converters;

/// <summary>
/// Converts RowActionType to tooltip text
/// </summary>
public class RowActionTooltipConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 3) return string.Empty;
        
        var actionType = values[0] as RowActionType? ?? RowActionType.Custom;
        var customText = values[1] as string;
        var isActive = values[2] as bool?;

        return actionType switch
        {
            RowActionType.Toggle => isActive == true ? "Deactivate this directive" : "Activate this directive",
            RowActionType.Edit => "Edit directive",
            RowActionType.Delete => "Delete directive (cannot be undone)",
            RowActionType.Custom => customText ?? "Custom action",
            _ => string.Empty
        };
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
