namespace CompanyName.ProjectName.Hosts.WPF.Framework.Configs;

/// <summary>
/// Defines what operations a directive type supports
/// </summary>
[Flags]
public enum DirectiveCapabilities
{
    None = 0,
    Create = 1,
    Edit = 2,
    Delete = 4,
    Toggle = 8,

    // Common combinations
    FullCrud = Create | Edit | Delete | Toggle,
    CreateOnly = Create | Toggle,
    CreateDelete = Create | Delete | Toggle
}
