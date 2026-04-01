namespace CompanyName.ProjectName.Hosts.WPF.Framework.Configs;

/// <summary>
/// Configuration for a row action button in DataGrid
/// </summary>
public class RowActionConfig
{
    public required string Id { get; init; }
    public RowActionType ActionType { get; init; }
    public string? Text { get; init; }                      // For custom text
    public required string CommandName { get; init; }
    public ActionButtonStyle Style { get; init; }
    public int Order { get; init; }
}
