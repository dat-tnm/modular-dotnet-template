namespace CompanyName.ProjectName.Hosts.WPF.Framework.Configs;

/// <summary>
/// Configuration for a search field in the search panel
/// </summary>
public class SearchFieldConfig
{
    public required string PropertyName { get; init; }      // ViewModel property to bind
    public required string Label { get; init; }             // Display label
    public string Placeholder { get; init; } = string.Empty; // Placeholder text
    public SearchFieldType FieldType { get; init; }         // TextBox, ComboBox, etc.
    public bool IsReadOnly { get; init; }                   // Fixed value (e.g., OpCode = "ASSYM")
    public string? FixedValue { get; init; }                // Value if read-only
    public int Width { get; init; } = 150;                  // Field width
    public int Order { get; init; }                         // Display order
    
    // For ComboBox type
    public IReadOnlyList<ComboBoxItemConfig>? ComboBoxItems { get; init; }
}
