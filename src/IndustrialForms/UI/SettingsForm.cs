using IndustrialForms.Core.Localization;
using IndustrialForms.Core.Theming;
using IndustrialForms.Framework;

namespace IndustrialForms.UI;

/// <summary>
/// 示例：设置窗体。演示如何用框架的多语言能力做双语设置项。
/// </summary>
public sealed class SettingsForm : BaseChildForm
{
    public SettingsForm(ILanguageService language)
        : base(language, "设置")
    {
    }

    protected override void InitializeUi()
    {
        Padding = new Padding(24);

        var title = new Label
        {
            Text = Language.GetText("设置"),
            Font = ThemeColors.UiFont(18f, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 0),
        };
        Controls.Add(title);

        var group = new GroupBox
        {
            Text = Language.GetText("语言 / Language"),
            Location = new Point(0, 48),
            Size = new Size(420, 120),
            Font = ThemeColors.UiFont(9f),
        };

        var chineseRadio = new RadioButton
        {
            Text = "中文",
            Location = new Point(16, 40),
            AutoSize = true,
            Checked = !Language.IsEnglish,
        };
        var englishRadio = new RadioButton
        {
            Text = "English",
            Location = new Point(120, 40),
            AutoSize = true,
            Checked = Language.IsEnglish,
        };

        chineseRadio.CheckedChanged += (_, _) =>
        {
            if (chineseRadio.Checked)
            {
                Language.IsEnglish = false;
            }
        };
        englishRadio.CheckedChanged += (_, _) =>
        {
            if (englishRadio.Checked)
            {
                Language.IsEnglish = true;
            }
        };

        group.Controls.Add(chineseRadio);
        group.Controls.Add(englishRadio);
        Controls.Add(group);
    }
}
