namespace CompanyName.ProjectName.Hosts.WPF.Framework.Configs;

/// <summary>
/// Configuration for a form field in edit/create dialogs
/// </summary>
public class FormFieldConfig
{
    public required string PropertyName { get; init; }      // ViewModel property
    public required string Label { get; init; }             // Field label
    public string HelpText { get; init; } = string.Empty;   // Helper text below field
    public string Placeholder { get; init; } = string.Empty; // Placeholder
    public FormFieldType FieldType { get; init; }           // TextBox, MultiLine, etc.
    public bool IsRequired { get; init; }                   // Show asterisk (*)
    public bool IsReadOnly { get; init; }                   // Disabled in all modes
    public bool IsReadOnlyInEdit { get; init; }             // Disabled only in edit mode
    public int Order { get; init; }                         // Display order
    public int MinHeight { get; init; }                     // For multiline textboxes
    public IReadOnlyList<ComboBoxItemConfig>? ComboBoxItems { get; init; } // For ComboBox type
}
