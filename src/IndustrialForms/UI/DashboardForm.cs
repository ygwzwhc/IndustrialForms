using IndustrialForms.Core.Localization;
using IndustrialForms.Core.Logging;
using IndustrialForms.Core.Storage;
using IndustrialForms.Core.Theming;
using IndustrialForms.Framework;

namespace IndustrialForms.UI;

/// <summary>
/// 示例：仪表盘窗体。用于演示框架能力——卡片式布局、Toast 提示、日志埋点，
/// 以及 SQLite 数据存储（参数 + 通信协议）的读取展示。
/// </summary>
public sealed class DashboardForm : BaseChildForm
{
    private readonly AppDatabase _db;
    private readonly ParameterRepository _parameters;
    private readonly ProtocolRepository _protocols;

    private Label _dbPathLabel = null!;
    private Label _paramCountLabel = null!;
    private Label _protocolLabel = null!;

    public DashboardForm(
        ILanguageService language,
        AppDatabase db,
        ParameterRepository parameters,
        ProtocolRepository protocols)
        : base(language, "仪表盘")
    {
        _db = db;
        _parameters = parameters;
        _protocols = protocols;
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

        BuildStoragePanel();
    }

    /// <summary>语言切换后，动态拼接的文本需手动重设（RefreshFormText 无法翻译含动态值的整串）。</summary>
    protected override void OnLanguageChanged()
    {
        base.OnLanguageChanged();
        RefreshStorageTexts();
    }

    private void BuildStoragePanel()
    {
        var group = new GroupBox
        {
            Text = Language.GetText("数据存储"),
            Font = ThemeColors.UiFont(9f),
            ForeColor = ThemeColors.TextPrimary,
            Location = new Point(0, 280),
            Size = new Size(560, 150),
        };

        _dbPathLabel = new Label
        {
            Font = ThemeColors.UiFont(9f),
            ForeColor = ThemeColors.TextSecondary,
            AutoSize = true,
            Location = new Point(16, 34),
        };
        _paramCountLabel = new Label
        {
            Font = ThemeColors.UiFont(9f),
            ForeColor = ThemeColors.TextSecondary,
            AutoSize = true,
            Location = new Point(16, 60),
        };
        _protocolLabel = new Label
        {
            Font = ThemeColors.UiFont(9f),
            ForeColor = ThemeColors.TextSecondary,
            AutoSize = true,
            Location = new Point(16, 86),
        };

        group.Controls.Add(_dbPathLabel);
        group.Controls.Add(_paramCountLabel);
        group.Controls.Add(_protocolLabel);
        Controls.Add(group);

        RefreshStorageTexts();
    }

    private void RefreshStorageTexts()
    {
        if (_dbPathLabel is null)
        {
            return;
        }

        var protocols = _protocols.GetAll();
        var first = protocols.Count > 0 ? protocols[0] : null;

        _dbPathLabel.Text = $"{Language.GetText("数据库文件")}：{_db.DatabasePath}";
        _paramCountLabel.Text = $"{Language.GetText("参数条目")}：{_parameters.Count()}";
        _protocolLabel.Text = first is null
            ? $"{Language.GetText("通信协议")}：0"
            : $"{Language.GetText("通信协议")}：{_protocols.Count()}（{first.Name} / {first.Transport}）";
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
