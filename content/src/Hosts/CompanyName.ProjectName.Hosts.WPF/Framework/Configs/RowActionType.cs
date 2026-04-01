namespace CompanyName.ProjectName.Hosts.WPF.Framework.Configs;

/// <summary>
/// Types of actions available for DataGrid rows
/// </summary>
public enum RowActionType
{
    Toggle,      // Activate/Deactivate (dynamic text)
    Edit,        // ✎ Edit
    Delete,      // 🗑 Delete
    Custom       // Custom action with specified text
}
