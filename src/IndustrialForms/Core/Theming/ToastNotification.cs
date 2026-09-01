using System.Drawing.Drawing2D;

namespace IndustrialForms.Core.Theming;

/// <summary>Toast 提示类型。</summary>
public enum ToastType
{
    Info,
    Success,
    Warning,
    Error
}

/// <summary>
/// 右下角滑入式 Toast 提示：无边框置顶、圆角渐变、平滑动画、重复提示过滤。
///
/// 通过静态方法 <see cref="Show"/> 调用，自动保证 UI 线程安全。
/// </summary>
public sealed class ToastNotification : Form
{
    private const int Width_ = 320;
    private const int Height_ = 80;
    private const int SlideMs = 160;
    private const int StayMs = 2400;
    private const int Step = 10;
    private const int ScreenMargin = 14;
    private const int Radius = 6;

    private static readonly object CountLock = new();
    private static int _activeCount;
    private static readonly Dictionary<string, DateTime> RecentCache = new();
    private static readonly object CacheLock = new();

    private static readonly Dictionary<ToastType, (Color From, Color To)> Gradients = new()
    {
        [ToastType.Info] = (Color.FromArgb(230, 240, 250), Color.FromArgb(30, 136, 229)),
        [ToastType.Success] = (Color.FromArgb(235, 249, 235), Color.FromArgb(76, 175, 80)),
        [ToastType.Warning] = (Color.FromArgb(255, 248, 225), Color.FromArgb(255, 152, 0)),
        [ToastType.Error] = (Color.FromArgb(255, 235, 238), Color.FromArgb(229, 57, 53)),
    };

    private readonly string _text;
    private readonly ToastType _type;
    private Point _target;

    private ToastNotification(string text, ToastType type)
    {
        _text = text;
        _type = type;

        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        DoubleBuffered = true;
        Size = new Size(Width_, Height_);
        BackColor = Gradients[type].From;

        Paint += OnPaint;
    }

    public static void Show(string text, ToastType type = ToastType.Info)
    {
        if (string.IsNullOrWhiteSpace(text) || IsDuplicate(text, type))
        {
            return;
        }

        var toast = new ToastNotification(text, type);

        if (Application.OpenForms.Count == 0)
        {
            toast.ShowAndRun();
            return;
        }

        var main = Application.OpenForms[0];
        if (main is null)
        {
            toast.ShowAndRun();
            return;
        }

        if (main.InvokeRequired)
        {
            main.Invoke((Action)toast.ShowAndRun);
        }
        else
        {
            toast.ShowAndRun();
        }
    }

    private async void ShowAndRun()
    {
        lock (CountLock)
        {
            _activeCount++;
        }

        var workArea = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1920, 1080);
        var targetX = workArea.Width - Width_ - ScreenMargin;
        var targetY = workArea.Height - Height_ - ScreenMargin - ((_activeCount - 1) * (Height_ + 10));
        _target = new Point(targetX, Math.Max(ScreenMargin, targetY));

        Region = new Region(CreateRoundedPath(ClientRectangle, Radius));
        Location = new Point(workArea.Width + ScreenMargin, _target.Y);
        Show();

        // 滑入 -> 停留 -> 滑出
        while (Left > _target.X)
        {
            Left = Math.Max(_target.X, Left - Step);
            await Task.Delay(SlideMs / Math.Max(1, (workArea.Width + ScreenMargin - _target.X) / Step));
        }

        await Task.Delay(StayMs);

        var offX = workArea.Width + ScreenMargin;
        while (Left < offX)
        {
            Left = Math.Min(offX, Left + Step);
            await Task.Delay(SlideMs / Math.Max(1, (offX - _target.X) / Step));
        }

        Close();
        lock (CountLock)
        {
            _activeCount--;
        }
    }

    private void OnPaint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var (from, to) = Gradients[_type];
        using var brush = new LinearGradientBrush(ClientRectangle, from, to, LinearGradientMode.Vertical);
        using var path = CreateRoundedPath(ClientRectangle, Radius);
        g.FillPath(brush, path);
        using var pen = new Pen(Color.FromArgb(40, to), 1f);
        g.DrawPath(pen, path);

        // 图标
        using var iconBrush = new SolidBrush(Color.FromArgb(45, 45, 45));
        using var iconFont = new Font("Segoe UI", 14f, FontStyle.Bold);
        g.DrawString(_type switch
        {
            ToastType.Success => "✓",
            ToastType.Warning => "!",
            ToastType.Error => "×",
            _ => "i",
        }, iconFont, iconBrush, 18, (Height_ - 22) / 2);

        // 文本
        using var textBrush = new SolidBrush(Color.FromArgb(50, 50, 50));
        using var textFont = new Font("Segoe UI", 9.5f);
        g.DrawString(_text, textFont, textBrush, new RectangleF(52, 12, Width_ - 64, Height_ - 24));
    }

    private static bool IsDuplicate(string text, ToastType type)
    {
        lock (CacheLock)
        {
            var now = DateTime.Now;
            var key = $"{type}_{text}";
            var expired = RecentCache.Where(kv => now - kv.Value > TimeSpan.FromSeconds(3)).Select(kv => kv.Key).ToList();
            foreach (var k in expired)
            {
                RecentCache.Remove(k);
            }

            if (RecentCache.ContainsKey(key))
            {
                return true;
            }

            RecentCache[key] = now;
            return false;
        }
    }

    private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
        path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
        path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
        path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
        path.CloseAllFigures();
        return path;
    }
}
