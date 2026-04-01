namespace CompanyName.ProjectName.Hosts.WPF.Framework.Configs;

/// <summary>
/// Configuration interface for a directive list view.
/// Defines all visual and behavioral aspects of the view.
/// </summary>
public interface IDirectiveViewConfig
{
    // View Identity
    string Title { get; }
    string Subtitle { get; }
    string Icon { get; }
    
    // Search Configuration
    IReadOnlyList<SearchFieldConfig> SearchFields { get; }
    
    // Grid Configuration
    IReadOnlyList<GridColumnConfig> GridColumns { get; }
    
    // Action Buttons (top bar buttons like "Create New")
    IReadOnlyList<ActionButtonConfig> ActionButtons { get; }
    
    // Row Actions (buttons in each DataGrid row)
    IReadOnlyList<RowActionConfig> RowActions { get; }
    
    // Form Configuration (for edit/create dialogs)
    IReadOnlyList<FormFieldConfig> FormFields { get; }
    
    // Capabilities (what operations are supported)
    DirectiveCapabilities Capabilities { get; }
}
