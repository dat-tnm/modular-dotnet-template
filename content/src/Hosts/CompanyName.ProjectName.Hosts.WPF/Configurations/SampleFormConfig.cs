using CompanyName.ProjectName.Hosts.WPF.Framework.Configs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Hosts.WPF.Configurations
{
    public class SampleFormConfig : IDirectiveViewConfig
    {
        public string Title => "Normal Directives";
        public string Subtitle => "On-Hold: Job & Partnum";
        public string Icon => "🔒";

        public DirectiveCapabilities Capabilities =>
            DirectiveCapabilities.Create | DirectiveCapabilities.Edit | DirectiveCapabilities.Toggle;

        public IReadOnlyList<SearchFieldConfig> SearchFields => new[]
        {
        new SearchFieldConfig
        {
            PropertyName = "Jobnum",
            Label = "Job Number",
            Placeholder = "e.g., J001234",
            FieldType = SearchFieldType.TextBox,
            Width = 150,
            Order = 1
        },
        new SearchFieldConfig
        {
            PropertyName = "OpCode",
            Label = "OpCode",
            Placeholder = "e.g., OP10",
            FieldType = SearchFieldType.TextBox,
            Width = 100,
            Order = 2
        },
        new SearchFieldConfig
        {
            PropertyName = "Requestors",
            Label = "Requestors",
            Placeholder = "Search by name",
            FieldType = SearchFieldType.TextBox,
            Width = 150,
            Order = 3
        },
        new SearchFieldConfig
        {
            PropertyName = "IsActive",
            Label = "Status",
            FieldType = SearchFieldType.ComboBox,
            Width = 120,
            Order = 4,
            ComboBoxItems = new[]
            {
                new ComboBoxItemConfig { DisplayText = "All", Value = null },
                new ComboBoxItemConfig { DisplayText = "Active", Value = true },
                new ComboBoxItemConfig { DisplayText = "Inactive", Value = false }
            }
        }
    };

        public IReadOnlyList<GridColumnConfig> GridColumns => new[]
        {
        new GridColumnConfig
        {
            PropertyName = "Jobnum",
            Header = "Job Number",
            Width = 120,
            Order = 1,
            ColumnType = GridColumnType.Text
        },
        new GridColumnConfig
        {
            PropertyName = "OpCode",
            Header = "OpCode",
            Width = 80,
            Order = 2,
            ColumnType = GridColumnType.Text
        },
        new GridColumnConfig
        {
            PropertyName = "OperationSequence",
            Header = "Seq",
            Width = 60,
            Order = 3,
            ColumnType = GridColumnType.Text
        },
        new GridColumnConfig
        {
            PropertyName = "Requestors",
            Header = "Requestors",
            Width = 150,
            Order = 4,
            ColumnType = GridColumnType.Text
        },
        new GridColumnConfig
        {
            PropertyName = "IsActive",
            Header = "Status",
            Width = 100,
            ColumnType = GridColumnType.StatusBadge,
            Order = 5
        },
        new GridColumnConfig
        {
            PropertyName = "AlertMessage",
            Header = "Message",
            IsWidthStar = true,
            ColumnType = GridColumnType.TruncatedText,
            Order = 6
        }
    };

        public IReadOnlyList<ActionButtonConfig> ActionButtons => new[]
        {
        new ActionButtonConfig
        {
            Id = "create",
            Text = "+ Create New Directive",
            Style = ActionButtonStyle.Primary,
            CommandName = "CreateCommand",
            Order = 1
        }
    };

        public IReadOnlyList<RowActionConfig> RowActions => new[]
        {
        new RowActionConfig
        {
            Id = "toggle",
            ActionType = RowActionType.Toggle,
            CommandName = "ToggleActiveCommand",
            Order = 1
        },
        new RowActionConfig
        {
            Id = "edit",
            ActionType = RowActionType.Edit,
            CommandName = "EditCommand",
            Order = 2
        }
    };

        public IReadOnlyList<FormFieldConfig> FormFields => new[]
        {
        new FormFieldConfig
        {
            PropertyName = "Jobnum",
            Label = "Job Number",
            HelpText = "Example: J001234",
            FieldType = FormFieldType.TextBox,
            IsRequired = true,
            IsReadOnlyInEdit = true,
            Order = 1
        },
        new FormFieldConfig
        {
            PropertyName = "OpCode",
            Label = "Operation Code",
            HelpText = "Example: OP10",
            FieldType = FormFieldType.TextBox,
            IsRequired = true,
            IsReadOnlyInEdit = true,
            Order = 2
        },
        new FormFieldConfig
        {
            PropertyName = "OperationSequence",
            Label = "Operation Sequence",
            HelpText = "Optional sequence number",
            Placeholder = "e.g., 1, 2, 3",
            FieldType = FormFieldType.TextBox,
            IsReadOnlyInEdit = true,
            Order = 3
        },
        new FormFieldConfig
        {
            PropertyName = "Requestors",
            Label = "Requestors",
            HelpText = "Comma-separated names (e.g., John Doe, Jane Smith)",
            Placeholder = "John Doe, Jane Smith",
            FieldType = FormFieldType.TextBox,
            IsRequired = true,
            Order = 4
        },
        new FormFieldConfig
        {
            PropertyName = "AlertMessage",
            Label = "Alert Message",
            HelpText = "Describe the reason for the hold",
            Placeholder = "Enter the reason for holding this job...",
            FieldType = FormFieldType.MultiLineTextBox,
            IsRequired = true,
            MinHeight = 80,
            Order = 5
        },
        new FormFieldConfig
        {
            PropertyName = "ReasonAlert",
            Label = "Additional Reason (Optional)",
            HelpText = "Any additional context or notes",
            Placeholder = "Optional additional details...",
            FieldType = FormFieldType.MultiLineTextBox,
            MinHeight = 60,
            Order = 6
        }
    };
    }

}
