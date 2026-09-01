using IndustrialForms.Core.Localization;
using IndustrialForms.Core.Logging;
using IndustrialForms.Core.Theming;
using IndustrialForms.Framework;

namespace IndustrialForms.UI;

/// <summary>
/// 主窗体：顶部菜单 + 左侧导航树 + 右侧内容区 + 底部状态栏。
///
/// 它是整个框架的组装点：把导航、子窗体管理、状态栏、多语言等能力
/// 拼装成一个可运行的上位机主界面骨架。
/// </summary>
public sealed class MainForm : Form
{
    private readonly ILanguageService _language;
    private readonly IServiceProvider _services;

    private ChildFormManager _childForms = null!;
    private NavigationTreeManager _navigation = null!;
    private StatusStripManager _status = null!;

    private ToolStripMenuItem _fileMenu = null!;
    private ToolStripMenuItem _viewMenu = null!;
    private ToolStripMenuItem _languageMenu = null!;
    private ToolStripMenuItem _helpMenu = null!;

    public MainForm(ILanguageService language, IServiceProvider services)
    {
        _language = language;
        _services = services;

        Text = "IndustrialForms";
        Size = new Size(1200, 760);
        MinimumSize = new Size(900, 600);
        StartPosition = FormStartPosition.CenterScreen;
        Font = ThemeColors.UiFont();
        BackColor = ThemeColors.WindowBackground;

        BuildMenu();
        BuildLayout();

        _language.LanguageChanged += RefreshOwnTexts;
        Load += (_, _) => Logger.Info("主窗体启动完成");
    }

    private void BuildMenu()
    {
        var menu = new MenuStrip { Font = ThemeColors.UiFont(9f) };

        _fileMenu = new ToolStripMenuItem(Language("文件"));
        _fileMenu.DropDownItems.Add(new ToolStripMenuItem(Language("退出"), null, (_, _) => Close()));

        _viewMenu = new ToolStripMenuItem(Language("视图"));
        _viewMenu.DropDownItems.Add(new ToolStripMenuItem(Language("日志查看器"), null, (_, _) => OpenForm(typeof(LogViewerForm))));

        _languageMenu = new ToolStripMenuItem(Language("语言"));
        _languageMenu.DropDownItems.Add(new ToolStripMenuItem("中文", null, (_, _) => SwitchLanguage(false)));
        _languageMenu.DropDownItems.Add(new ToolStripMenuItem("English", null, (_, _) => SwitchLanguage(true)));

        _helpMenu = new ToolStripMenuItem(Language("帮助"));
        _helpMenu.DropDownItems.Add(new ToolStripMenuItem(Language("关于"), null, (_, _) => OpenForm(typeof(AboutForm))));

        menu.Items.AddRange(new ToolStripItem[] { _fileMenu, _viewMenu, _languageMenu, _helpMenu });
        MainMenuStrip = menu;
        Controls.Add(menu);
    }

    private void BuildLayout()
    {
        var body = new Panel
        {
            Dock = DockStyle.Fill,
            Top = MainMenuStrip!.Height,
        };

        // 左侧导航栏
        var sidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 210,
            BackColor = ThemeColors.SidebarBackground,
            Padding = new Padding(0, 16, 0, 0),
        };

        var brand = new Label
        {
            Text = "IndustrialForms",
            Font = ThemeColors.UiFont(13f, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(20, 4),
        };
        sidebar.Controls.Add(brand);

        var tree = new TreeView
        {
            Dock = DockStyle.Fill,
            Top = 48,
        };
        sidebar.Controls.Add(tree);

        // 右侧内容区
        var content = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = ThemeColors.WindowBackground,
        };

        body.Controls.Add(content);
        body.Controls.Add(sidebar);

        // 底部状态栏
        var statusStrip = new StatusStrip { SizingGrip = false };

        Controls.Add(body);
        Controls.Add(statusStrip);
        body.BringToFront();

        // 组装框架组件
        _childForms = new ChildFormManager(_services, content);
        _navigation = new NavigationTreeManager(tree);
        _navigation.RegisterNode("dashboard", "仪表盘", typeof(DashboardForm));
        _navigation.RegisterNode("settings", "设置", typeof(SettingsForm));
        _navigation.RegisterNode("logs", "日志查看器", typeof(LogViewerForm));
        _navigation.RegisterNode("about", "关于", typeof(AboutForm));
        _navigation.NodeSelected += OpenForm;

        _status = new StatusStripManager(statusStrip);

        // 默认打开仪表盘
        tree.SelectedNode = tree.Nodes[0];
    }

    private void OpenForm(Type formType)
    {
        _childForms.Open(formType);
        _status.SetStatus($"{_language.GetText("当前页面")}：{_language.GetText(ResolveNodeText(formType))}");
    }

    private static string ResolveNodeText(Type formType)
    {
        return formType.Name switch
        {
            nameof(DashboardForm) => "仪表盘",
            nameof(SettingsForm) => "设置",
            nameof(LogViewerForm) => "日志查看器",
            nameof(AboutForm) => "关于",
            _ => formType.Name,
        };
    }

    private void SwitchLanguage(bool isEnglish) => _language.IsEnglish = isEnglish;

    private void RefreshOwnTexts()
    {
        _fileMenu.Text = Language("文件");
        _viewMenu.Text = Language("视图");
        _languageMenu.Text = Language("语言");
        _helpMenu.Text = Language("帮助");
        _navigation.RefreshTexts(_language);
    }

    private string Language(string cnText) => _language.GetText(cnText);
}
