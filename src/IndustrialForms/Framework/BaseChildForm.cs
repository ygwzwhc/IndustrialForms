using System.ComponentModel;
using IndustrialForms.Common;
using IndustrialForms.Core.Localization;
using IndustrialForms.Core.Logging;
using IndustrialForms.Core.Theming;

namespace IndustrialForms.Framework;

/// <summary>
/// 子窗体基类：为所有业务窗体提供统一的基础能力，让业务代码专注于自身逻辑。
///
/// 内置能力：
/// 1. 多语言联动 —— 语言切换时自动刷新自身及所有子控件文本；
/// 2. 异步加载信号 —— 通过 <see cref="LoadCompletedTask"/> 感知窗体加载完成，便于父窗体等待；
/// 3. 跨线程安全 —— <see cref="InvokeIfRequired"/> 统一处理线程切换；
/// 4. 规范资源释放 —— 重写 Dispose，集中释放订阅。
/// </summary>
public class BaseChildForm : Form
{
    private readonly string _formNameCn;
    private readonly TaskCompletionSource<bool> _loadCompleted =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private bool _disposed;

    /// <summary>当前窗体使用的多语言服务。</summary>
    protected readonly ILanguageService Language;

    /// <summary>窗体是否已完成加载。</summary>
    public bool IsFormLoaded => _loadCompleted.Task.IsCompletedSuccessfully;

    /// <summary>异步加载完成信号。</summary>
    public Task LoadCompletedTask => _loadCompleted.Task;

    /// <summary>当前是否处于设计器模式。</summary>
    protected bool IsDesignMode => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

    protected BaseChildForm(ILanguageService language, string formNameCn)
    {
        Language = language ?? throw new ArgumentNullException(nameof(language));
        _formNameCn = string.IsNullOrWhiteSpace(formNameCn) ? "子窗体" : formNameCn;

        Font = ThemeColors.UiFont();
        DoubleBuffered = true;
        BackColor = ThemeColors.WindowBackground;
        StartPosition = FormStartPosition.CenterParent;
        AutoScaleMode = AutoScaleMode.Font;
        Text = _formNameCn;

        InitializeUi();

        Load += OnLoad;
        FormClosed += OnFormClosed;
        if (!IsDesignMode)
        {
            Language.LanguageChanged += OnLanguageChanged;
        }
    }

    /// <summary>供子类在构造函数中构建界面，替代设计器生成的 InitializeComponent。</summary>
    protected virtual void InitializeUi()
    {
    }

    /// <summary>语言切换时触发，刷新自身及子控件文本。</summary>
    protected virtual void OnLanguageChanged()
    {
        if (IsDesignMode || IsDisposed)
        {
            return;
        }

        Text = Language.GetText(_formNameCn);
        Language.RefreshFormText(this);
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        if (IsDesignMode)
        {
            _loadCompleted.TrySetResult(true);
            return;
        }

        try
        {
            OnFormLoaded();
            _loadCompleted.TrySetResult(true);
        }
        catch (Exception ex)
        {
            _loadCompleted.TrySetException(ex);
            Logger.Error($"窗体[{_formNameCn}]加载失败", ex);
        }
    }

    /// <summary>窗体加载完成后的业务钩子，供子类重写。</summary>
    protected virtual void OnFormLoaded()
    {
    }

    private void OnFormClosed(object? sender, FormClosedEventArgs e)
    {
        _loadCompleted.TrySetCanceled();

        if (IsDesignMode)
        {
            return;
        }

        Language.LanguageChanged -= OnLanguageChanged;
    }

    /// <summary>跨线程安全执行 UI 操作。</summary>
    protected void InvokeIfRequired(Action action) => this.InvokeIfRequired(action);

    /// <summary>递归查找指定名称的控件。</summary>
    protected IEnumerable<Control> FindControlsByName(string name) =>
        this.FindControlsByName(name);

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing && !IsDesignMode)
        {
            _loadCompleted.TrySetCanceled();
            Language.LanguageChanged -= OnLanguageChanged;
            Language.ClearFormCache(this);
        }

        _disposed = true;
        base.Dispose(disposing);
    }
}
