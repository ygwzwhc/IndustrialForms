using IndustrialForms.Core.Localization;
using IndustrialForms.Core.Theming;
using IndustrialForms.Framework;

namespace IndustrialForms.UI;

/// <summary>
/// 示例：关于窗体。展示框架定位与技术栈。
/// </summary>
public sealed class AboutForm : BaseChildForm
{
    public AboutForm(ILanguageService language)
        : base(language, "关于")
    {
    }

    protected override void InitializeUi()
    {
        Padding = new Padding(24);

        var logo = new Label
        {
            Text = "IndustrialForms",
            Font = ThemeColors.UiFont(22f, FontStyle.Bold),
            ForeColor = ThemeColors.Primary,
            AutoSize = true,
            Location = new Point(0, 8),
        };

        var tagline = new Label
        {
            Text = Language.GetText("工业级 WinForms 上位机 UI 框架"),
            Font = ThemeColors.UiFont(10f),
            ForeColor = ThemeColors.TextSecondary,
            AutoSize = true,
            Location = new Point(0, 52),
        };

        var info = new Label
        {
            Text = ".NET 10 · WinForms · DI · 多语言 · 日志",
            Font = ThemeColors.UiFont(9f),
            ForeColor = ThemeColors.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 88),
        };

        Controls.Add(logo);
        Controls.Add(tagline);
        Controls.Add(info);
    }
}
