using System.Diagnostics;
using IndustrialForms.Core.Localization;
using IndustrialForms.Core.Logging;
using IndustrialForms.Core.Theming;
using IndustrialForms.Framework;

namespace IndustrialForms.UI;

/// <summary>
/// 示例：关于窗体。展示框架定位、技术栈、联系信息与仓库地址。
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

        var stack = new Label
        {
            Text = ".NET 10 · WinForms · DI · 多语言 · 日志 · SQLite",
            Font = ThemeColors.UiFont(9f),
            ForeColor = ThemeColors.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 86),
        };

        var description = new Label
        {
            Text = Language.GetText("通用型上位机 UI 框架骨架，不含原步进电机项目的业务参数与私有协议。"),
            Font = ThemeColors.UiFont(9f),
            ForeColor = ThemeColors.TextSecondary,
            AutoSize = true,
            Location = new Point(0, 120),
            MaximumSize = new Size(520, 0),
        };

        var contactLabel = new Label
        {
            Text = Language.GetText("联系作者") + "：",
            Font = ThemeColors.UiFont(9f, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 178),
        };

        var emailLink = new LinkLabel
        {
            Text = "wanghaochenemail@163.com",
            Font = ThemeColors.UiFont(9f),
            Location = new Point(80, 178),
            AutoSize = true,
            LinkColor = ThemeColors.Primary,
        };
        emailLink.LinkClicked += (_, _) =>
        {
            try
            {
                Process.Start(new ProcessStartInfo("mailto:wanghaochenemail@163.com") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Logger.Warn($"{Language.GetText("无法打开邮件客户端")}：{ex.Message}");
            }
        };

        var githubLabel = new Label
        {
            Text = "GitHub：",
            Font = ThemeColors.UiFont(9f, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 206),
        };

        var githubLink = new LinkLabel
        {
            Text = "github.com/ygwzwhc/IndustrialForms",
            Font = ThemeColors.UiFont(9f),
            Location = new Point(80, 206),
            AutoSize = true,
            LinkColor = ThemeColors.Primary,
        };
        githubLink.LinkClicked += (_, _) =>
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/ygwzwhc/IndustrialForms") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Logger.Warn($"{Language.GetText("无法打开浏览器")}：{ex.Message}");
            }
        };

        Controls.Add(logo);
        Controls.Add(tagline);
        Controls.Add(stack);
        Controls.Add(description);
        Controls.Add(contactLabel);
        Controls.Add(emailLink);
        Controls.Add(githubLabel);
        Controls.Add(githubLink);
    }
}
