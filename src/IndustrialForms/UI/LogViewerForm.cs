using IndustrialForms.Common;
using IndustrialForms.Core.Localization;
using IndustrialForms.Core.Logging;
using IndustrialForms.Core.Theming;
using IndustrialForms.Framework;

namespace IndustrialForms.UI;

/// <summary>
/// 日志查看器：实时展示进程日志，支持清空、自动滚动与级别着色。
///
/// 通过订阅 <see cref="Logger.LogWritten"/> 工作，自身无业务依赖。
/// </summary>
public sealed class LogViewerForm : BaseChildForm
{
    private readonly RichTextBox _logBox = new();
    private readonly CheckBox _autoScroll = new() { Text = "自动滚动", Checked = true };

    public LogViewerForm(ILanguageService language)
        : base(language, "日志查看器")
    {
    }

    protected override void InitializeUi()
    {
        var toolbar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(12, 8, 12, 8),
        };

        var clearButton = new Button
        {
            Text = "清空",
            Width = 80,
            Height = 30,
            FlatStyle = FlatStyle.Flat,
            BackColor = ThemeColors.Primary,
            ForeColor = Color.White,
        };
        clearButton.FlatAppearance.BorderSize = 0;
        clearButton.Click += (_, _) => _logBox.Clear();

        _autoScroll.AutoSize = true;
        _autoScroll.Font = ThemeColors.UiFont();
        toolbar.Controls.Add(clearButton);
        toolbar.Controls.Add(_autoScroll);
        _autoScroll.Location = new Point(clearButton.Right + 16, 14);

        _logBox.Dock = DockStyle.Fill;
        _logBox.ReadOnly = true;
        _logBox.BackColor = Color.FromArgb(30, 30, 30);
        _logBox.ForeColor = Color.FromArgb(220, 220, 220);
        _logBox.Font = new Font("Consolas", 9.5f);
        _logBox.WordWrap = false;
        _logBox.BorderStyle = BorderStyle.None;

        Controls.Add(_logBox);
        Controls.Add(toolbar);
        _logBox.BringToFront();
        toolbar.BringToFront();

        Logger.LogWritten += OnLogWritten;
    }

    protected override void OnFormLoaded()
    {
        // 回填历史日志，让查看器打开即见启动以来的记录。
        foreach (var line in Logger.GetHistoryLogs())
        {
            AppendLine(line);
        }
    }

    private void OnLogWritten(string line)
    {
        this.InvokeIfRequired(() => AppendLine(line));
    }

    private void AppendLine(string line)
    {
        var start = _logBox.TextLength;
        _logBox.AppendText(line + Environment.NewLine);

        _logBox.SelectionStart = start;
        _logBox.SelectionLength = line.Length;
        _logBox.SelectionColor = ResolveColor(line);

        if (_autoScroll.Checked)
        {
            _logBox.SelectionStart = _logBox.TextLength;
            _logBox.ScrollToCaret();
        }
    }

    private static Color ResolveColor(string line)
    {
        if (line.Contains("[ERROR]"))
        {
            return Color.FromArgb(255, 110, 110);
        }

        if (line.Contains("[WARN]"))
        {
            return Color.FromArgb(255, 200, 90);
        }

        if (line.Contains("[DEBUG]"))
        {
            return Color.FromArgb(150, 150, 150);
        }

        return Color.FromArgb(200, 220, 200);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Logger.LogWritten -= OnLogWritten;
        }

        base.Dispose(disposing);
    }
}
