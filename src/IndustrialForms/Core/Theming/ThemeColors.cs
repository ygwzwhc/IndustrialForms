namespace IndustrialForms.Core.Theming;

/// <summary>
/// 全局主题颜色令牌。集中定义配色，UI 各处引用同一套颜色，
/// 便于统一调整风格、保持视觉一致性。
/// </summary>
public static class ThemeColors
{
    // 主色：工业蓝
    public static Color Primary { get; } = Color.FromArgb(30, 136, 229);
    public static Color PrimaryDark { get; } = Color.FromArgb(21, 101, 192);

    // 语义色
    public static Color Success { get; } = Color.FromArgb(76, 175, 80);
    public static Color Warning { get; } = Color.FromArgb(255, 152, 0);
    public static Color Error { get; } = Color.FromArgb(229, 57, 53);

    // 背景与文字
    public static Color WindowBackground { get; } = Color.FromArgb(250, 250, 252);
    public static Color PanelBackground { get; } = Color.White;
    public static Color SidebarBackground { get; } = Color.FromArgb(38, 50, 56);
    public static Color TextPrimary { get; } = Color.FromArgb(33, 33, 33);
    public static Color TextSecondary { get; } = Color.FromArgb(117, 117, 117);

    /// <summary>框架默认字体（中英文混排友好的现代无衬线字体）。</summary>
    public static Font UiFont(float size = 9f, FontStyle style = FontStyle.Regular) =>
        new("Microsoft YaHei UI", size, style);
}
