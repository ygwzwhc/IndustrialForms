using IndustrialForms.Core.Theming;

namespace IndustrialForms.Framework;

/// <summary>
/// 底部状态栏管理器：集中维护状态信息与时钟显示。
/// </summary>
public sealed class StatusStripManager : IDisposable
{
    private readonly ToolStripStatusLabel _statusLabel;
    private readonly ToolStripStatusLabel _timeLabel;
    private readonly System.Windows.Forms.Timer _clock = new();

    public StatusStripManager(StatusStrip strip)
    {
        _statusLabel = new ToolStripStatusLabel
        {
            Text = "就绪",
            Spring = true,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = ThemeColors.TextSecondary,
        };
        _timeLabel = new ToolStripStatusLabel
        {
            Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            ForeColor = ThemeColors.TextSecondary,
        };

        strip.Items.Add(_statusLabel);
        strip.Items.Add(_timeLabel);

        _clock.Interval = 1000;
        _clock.Tick += (_, _) => _timeLabel.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _clock.Start();
    }

    /// <summary>更新状态栏主状态文本。</summary>
    public void SetStatus(string text, Color? color = null)
    {
        _statusLabel.Text = text;
        _statusLabel.ForeColor = color ?? ThemeColors.TextSecondary;
    }

    public void Dispose() => _clock.Dispose();
}
