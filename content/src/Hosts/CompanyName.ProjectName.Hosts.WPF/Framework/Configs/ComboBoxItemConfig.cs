namespace CompanyName.ProjectName.Hosts.WPF.Framework.Configs;

/// <summary>
/// Configuration for a ComboBox item
/// </summary>
public class ComboBoxItemConfig
{
    public required string DisplayText { get; init; }
    public object? Value { get; init; }
}
