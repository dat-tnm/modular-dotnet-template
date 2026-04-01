using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using System.Reflection;

namespace CompanyName.ProjectName.Hosts.WPF.Converters;

/// <summary>
/// Converts CommandName string to actual ICommand from ViewModel
/// </summary>
public class RowActionCommandConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2) return null;
        
        var commandName = values[0] as string;
        var viewModel = values[1];
        
        if (string.IsNullOrEmpty(commandName) || viewModel == null)
            return null;

        // Get command property by reflection
        var property = viewModel.GetType().GetProperty(commandName, BindingFlags.Public | BindingFlags.Instance);
        if (property == null) return null;

        return property.GetValue(viewModel) as ICommand;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
