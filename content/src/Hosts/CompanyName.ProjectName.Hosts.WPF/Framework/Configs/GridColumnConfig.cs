namespace CompanyName.ProjectName.Hosts.WPF.Framework.Configs;

/// <summary>
/// Configuration for a DataGrid column
/// </summary>
public class GridColumnConfig
{
    public required string PropertyName { get; init; }      // DTO property to bind
    public required string Header { get; init; }            // Column header
    public int Width { get; init; }                         // Column width (ignored if IsWidthStar)
    public bool IsWidthStar { get; init; }                  // Use * width (fill remaining space)
    public GridColumnType ColumnType { get; init; }         // Text, StatusBadge, etc.
    public string? ConverterKey { get; init; }              // Converter resource key
    public int Order { get; init; }                         // Display order
    public bool IsSortable { get; init; } = true;
}
