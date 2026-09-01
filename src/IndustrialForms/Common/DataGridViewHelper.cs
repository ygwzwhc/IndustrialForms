using IndustrialForms.Core.Theming;

namespace IndustrialForms.Common;

/// <summary>
/// DataGridView 样式工具：统一表格外观，避免在每个窗体重复样式代码。
/// </summary>
public static class DataGridViewHelper
{
    /// <summary>应用框架统一表格样式。</summary>
    public static void ApplyDefaultStyle(DataGridView grid)
    {
        grid.BackgroundColor = ThemeColors.PanelBackground;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.GridColor = Color.FromArgb(235, 235, 238);
        grid.EnableHeadersVisualStyles = false;

        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(247, 248, 250),
            ForeColor = ThemeColors.TextPrimary,
            Font = ThemeColors.UiFont(9f, FontStyle.Bold),
            Alignment = DataGridViewContentAlignment.MiddleLeft,
        };
        grid.ColumnHeadersHeight = 40;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

        grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = ThemeColors.PanelBackground,
            ForeColor = ThemeColors.TextPrimary,
            SelectionBackColor = Color.FromArgb(225, 240, 255),
            SelectionForeColor = ThemeColors.TextPrimary,
            Font = ThemeColors.UiFont(9f),
            Padding = new Padding(4, 0, 0, 0),
        };

        grid.RowTemplate.Height = 36;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToResizeRows = false;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    }
}
