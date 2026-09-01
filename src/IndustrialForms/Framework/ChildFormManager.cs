using Microsoft.Extensions.DependencyInjection;

namespace IndustrialForms.Framework;

/// <summary>
/// 子窗体管理器：负责业务子窗体的创建、复用与切换。
///
/// 核心约定：同一类型的子窗体全局只保留一个实例，重复打开时直接切换到已有实例，
/// 避免用户反复点击导航导致大量窗体叠加。
/// </summary>
public sealed class ChildFormManager
{
    private readonly IServiceProvider _services;
    private readonly Panel _host;
    private readonly Dictionary<Type, Form> _cache = new();

    public ChildFormManager(IServiceProvider services, Panel host)
    {
        _services = services;
        _host = host;
    }

    /// <summary>打开指定类型的子窗体（单例复用，嵌入宿主面板）。</summary>
    public TForm Open<TForm>() where TForm : Form => (TForm)Open(typeof(TForm));

    /// <summary>按类型打开子窗体（供导航树等运行时场景使用）。</summary>
    public Form Open(Type formType)
    {
        if (_cache.TryGetValue(formType, out var existing) && !existing.IsDisposed)
        {
            BringToFront(existing);
            return existing;
        }

        var form = (Form)_services.GetRequiredService(formType);
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;

        _host.Controls.Clear();
        _host.Controls.Add(form);
        form.Show();
        form.BringToFront();

        _cache[formType] = form;
        return form;
    }

    /// <summary>关闭所有已打开的子窗体。</summary>
    public void CloseAll()
    {
        foreach (var form in _cache.Values)
        {
            form.Close();
        }

        _cache.Clear();
        _host.Controls.Clear();
    }

    private static void BringToFront(Form form)
    {
        if (form.Parent is null)
        {
            return;
        }

        form.Parent.Controls.Clear();
        form.Parent.Controls.Add(form);
        form.BringToFront();
    }
}
