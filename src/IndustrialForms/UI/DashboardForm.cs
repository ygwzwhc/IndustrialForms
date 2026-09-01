using IndustrialForms.Core.Localization;
using IndustrialForms.Core.Logging;
using IndustrialForms.Core.Theming;
using IndustrialForms.Framework;

namespace IndustrialForms.UI;

/// <summary>
/// 示例：仪表盘窗体。用于演示框架能力——卡片式布局、Toast 提示、日志埋点。
/// 数据均为静态示例，仅用于展示界面组织方式。
/// </summary>
public sealed class DashboardForm : BaseChildForm
{
    public DashboardForm(ILanguageService language)
        : base(language, "仪表盘")
    {
    }

    protected override void InitializeUi()
    {
        Padding = new Padding(24);

        var title = new Label
        {
            Text = Language.GetText("仪表盘"),
            Font = ThemeColors.UiFont(18f, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 0),
        };

        var subtitle = new Label
        {
            Text = Language.GetText("框架运行概况（示例数据）"),
            Font = ThemeColors.UiFont(9f),
            ForeColor = ThemeColors.TextSecondary,
            AutoSize = true,
            Location = new Point(0, 36),
        };

        Controls.Add(title);
        Controls.Add(subtitle);

        var cards = new[]
        {
            ("设备总数", "128", ThemeColors.Primary),
            ("在线设备", "112", ThemeColors.Success),
            ("告警数", "3", ThemeColors.Warning),
            ("今日指令", "8,462", ThemeColors.PrimaryDark),
        };

        var x = 0;
        foreach (var (label, value, color) in cards)
        {
            var card = CreateCard(label, value, color);
            card.Location = new Point(x, 72);
            Controls.Add(card);
            x += card.Width + 16;
        }

        var actionButton = new Button
        {
            Text = Language.GetText("触发一条提示"),
            Size = new Size(140, 36),
            Location = new Point(0, 220),
            FlatStyle = FlatStyle.Flat,
            BackColor = ThemeColors.Primary,
            ForeColor = Color.White,
        };
        actionButton.FlatAppearance.BorderSize = 0;
        actionButton.Click += (_, _) =>
        {
            ToastNotification.Show(Language.GetText("操作成功"), ToastType.Success);
            Logger.Info("用户在仪表盘触发了提示按钮");
        };

        Controls.Add(actionButton);
    }

    private static Panel CreateCard(string label, string value, Color accent)
    {
        var card = new Panel
        {
            Size = new Size(170, 120),
            BackColor = ThemeColors.PanelBackground,
        };

        var valueLabel = new Label
        {
            Text = value,
            Font = ThemeColors.UiFont(24f, FontStyle.Bold),
            ForeColor = accent,
            Location = new Point(16, 16),
            AutoSize = true,
        };

        var nameLabel = new Label
        {
            Text = label,
            Font = ThemeColors.UiFont(9f),
            ForeColor = ThemeColors.TextSecondary,
            Location = new Point(16, 74),
            AutoSize = true,
        };

        card.Controls.Add(valueLabel);
        card.Controls.Add(nameLabel);
        return card;
    }
}
