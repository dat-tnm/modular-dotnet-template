namespace CompanyName.ProjectName.Hosts.WPF.Framework.Configs;

/// <summary>
/// Configuration for an action button (e.g., "Create New")
/// </summary>
public class ActionButtonConfig
{
    public required string Id { get; init; }                // Unique identifier
    public required string Text { get; init; }              // Button text
    public string? Icon { get; init; }                      // Optional icon
    public ActionButtonStyle Style { get; init; }           // Primary, Secondary, Danger
    public required string CommandName { get; init; }       // Command to execute
    public object? CommandParameter { get; init; }          // Optional parameter
    public int Order { get; init; }                         // Display order
}
