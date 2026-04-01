using CompanyName.ProjectName.Hosts.WPF.Framework.Configs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace CompanyName.ProjectName.Hosts.WPF.Framework.Behaviors;


/// <summary>
/// Attached behavior that dynamically generates DataGrid columns from configuration
/// </summary>
public static class DataGridColumnsBehavior
{
    public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.RegisterAttached(
            "Columns",
            typeof(IEnumerable<GridColumnConfig>),
            typeof(DataGridColumnsBehavior),
            new PropertyMetadata(null, OnColumnsChanged));

    public static IEnumerable<GridColumnConfig>? GetColumns(DependencyObject obj)
    {
        return (IEnumerable<GridColumnConfig>?)obj.GetValue(ColumnsProperty);
    }

    public static void SetColumns(DependencyObject obj, IEnumerable<GridColumnConfig>? value)
    {
        obj.SetValue(ColumnsProperty, value);
    }

    private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGrid dataGrid) return;

        // Unsubscribe from old collection
        if (e.OldValue is INotifyCollectionChanged oldCollection)
        {
            oldCollection.CollectionChanged -= (s, args) => RegenerateColumns(dataGrid, e.NewValue as IEnumerable<GridColumnConfig>);
        }

        // Subscribe to new collection
        if (e.NewValue is INotifyCollectionChanged newCollection)
        {
            newCollection.CollectionChanged += (s, args) => RegenerateColumns(dataGrid, e.NewValue as IEnumerable<GridColumnConfig>);
        }

        // Generate columns
        RegenerateColumns(dataGrid, e.NewValue as IEnumerable<GridColumnConfig>);
    }

    private static void RegenerateColumns(DataGrid dataGrid, IEnumerable<GridColumnConfig>? configs)
    {
        if (configs == null) return;

        dataGrid.Columns.Clear();

        foreach (var config in configs.OrderBy(c => c.Order))
        {
            var column = CreateColumn(config);
            dataGrid.Columns.Add(column);
        }

        // Add Actions column last
        AddActionsColumn(dataGrid);
    }

    private static DataGridColumn CreateColumn(GridColumnConfig config)
    {
        return config.ColumnType switch
        {
            GridColumnType.Text => CreateTextColumn(config),
            GridColumnType.StatusBadge => CreateStatusBadgeColumn(config),
            GridColumnType.TruncatedText => CreateTruncatedTextColumn(config),
            GridColumnType.DateTime => CreateDateTimeColumn(config),
            GridColumnType.Custom => CreateCustomColumn(config),
            _ => throw new NotSupportedException($"Column type {config.ColumnType} not supported")
        };
    }

    private static DataGridTextColumn CreateTextColumn(GridColumnConfig config)
    {
        var column = new DataGridTextColumn
        {
            Header = config.Header,
            Binding = new Binding(config.PropertyName),
            CanUserSort = config.IsSortable
        };

        SetColumnWidth(column, config);

        if (!string.IsNullOrEmpty(config.ConverterKey))
        {
            // Apply converter if specified
            var binding = column.Binding as Binding;
            if (binding != null)
            {
                binding.Converter = Application.Current.TryFindResource(config.ConverterKey) as IValueConverter;
            }
        }

        return column;
    }

    private static DataGridTemplateColumn CreateStatusBadgeColumn(GridColumnConfig config)
    {
        var column = new DataGridTemplateColumn
        {
            Header = config.Header,
            CanUserSort = config.IsSortable
        };

        SetColumnWidth(column, config);

        // Create template for status badge
        var template = new DataTemplate();
        var factory = new FrameworkElementFactory(typeof(Border));
        factory.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
        factory.SetValue(Border.PaddingProperty, new Thickness(8, 4, 8, 4));
        factory.SetValue(Border.HorizontalAlignmentProperty, HorizontalAlignment.Center);

        // Bind background color based on IsActive
        var bgBinding = new Binding(config.PropertyName)
        {
            Converter = new BoolToStatusBackgroundConverter()
        };
        factory.SetBinding(Border.BackgroundProperty, bgBinding);

        // TextBlock inside border
        var textFactory = new FrameworkElementFactory(typeof(TextBlock));
        var textBinding = new Binding(config.PropertyName)
        {
            Converter = Application.Current.TryFindResource("BoolToActiveTextConverter") as IValueConverter
        };
        textFactory.SetBinding(TextBlock.TextProperty, textBinding);
        textFactory.SetValue(TextBlock.FontSizeProperty, 12.0);
        textFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.SemiBold);

        // Bind foreground color
        var fgBinding = new Binding(config.PropertyName)
        {
            Converter = new BoolToStatusForegroundConverter()
        };
        textFactory.SetBinding(TextBlock.ForegroundProperty, fgBinding);

        factory.AppendChild(textFactory);
        template.VisualTree = factory;
        column.CellTemplate = template;

        return column;
    }

    private static DataGridTextColumn CreateTruncatedTextColumn(GridColumnConfig config)
    {
        var column = CreateTextColumn(config);

        // Add truncation converter if not already specified
        if (string.IsNullOrEmpty(config.ConverterKey))
        {
            var binding = column.Binding as Binding;
            if (binding != null)
            {
                binding.Converter = Application.Current.TryFindResource("TruncateTextConverter") as IValueConverter;
            }
        }

        return column;
    }

    private static DataGridTextColumn CreateDateTimeColumn(GridColumnConfig config)
    {
        var column = CreateTextColumn(config);

        // Apply date format
        var binding = column.Binding as Binding;
        if (binding != null)
        {
            binding.StringFormat = "yyyy-MM-dd HH:mm";
        }

        return column;
    }

    private static DataGridTemplateColumn CreateCustomColumn(GridColumnConfig config)
    {
        // For custom columns, developer must provide template via resources
        var column = new DataGridTemplateColumn
        {
            Header = config.Header,
            CanUserSort = config.IsSortable
        };

        SetColumnWidth(column, config);

        // Try to find custom template
        var templateKey = $"{config.PropertyName}ColumnTemplate";
        if (Application.Current.TryFindResource(templateKey) is DataTemplate template)
        {
            column.CellTemplate = template;
        }

        return column;
    }

    private static void AddActionsColumn(DataGrid dataGrid)
    {
        // Actions column will be added dynamically from row actions config
        // This is handled separately in the GenericDirectiveListView
    }

    private static void SetColumnWidth(DataGridColumn column, GridColumnConfig config)
    {
        if (config.IsWidthStar)
        {
            column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        }
        else
        {
            column.Width = new DataGridLength(config.Width);
        }
    }

    // Helper converters for status badge
    private class BoolToStatusBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive
                    ? new SolidColorBrush(Color.FromRgb(232, 245, 233)) // Green background
                    : new SolidColorBrush(Color.FromRgb(255, 235, 238)); // Red background
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    private class BoolToStatusForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive
                    ? new SolidColorBrush(Color.FromRgb(76, 175, 80))  // Green text
                    : new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Red text
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
