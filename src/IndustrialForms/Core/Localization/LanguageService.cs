using System.Collections.Concurrent;

namespace IndustrialForms.Core.Localization;

/// <summary>
/// 极简多语言服务：以中文为基准 key，映射到英文。
///
/// 刷新策略：首次遇到控件时，把它的原始文本缓存到 <see cref="Control.Tag"/>，
/// 之后切换语言只需用缓存的中文重新查询映射，避免丢失原始文案。
/// </summary>
public sealed class LanguageService : ILanguageService
{
    private readonly ConcurrentDictionary<string, string> _mapping = new(StringComparer.Ordinal);
    private bool _isEnglish;

    /// <summary>是否为英文模式。赋值时若值发生变化，自动触发 <see cref="LanguageChanged"/>。</summary>
    public bool IsEnglish
    {
        get => _isEnglish;
        set
        {
            if (_isEnglish == value)
            {
                return;
            }

            _isEnglish = value;
            LanguageChanged?.Invoke();
        }
    }

    public event Action? LanguageChanged;

    public void AddMapping(string cnText, string enText)
    {
        if (string.IsNullOrWhiteSpace(cnText))
        {
            return;
        }

        _mapping[cnText] = enText;
    }

    public string GetText(string cnText)
    {
        if (string.IsNullOrEmpty(cnText))
        {
            return cnText;
        }

        if (!IsEnglish)
        {
            return cnText;
        }

        return _mapping.TryGetValue(cnText, out var en) ? en : cnText;
    }

    public void RefreshFormText(Form form)
    {
        foreach (var control in EnumerateControls(form))
        {
            ApplyToControl(control);
        }
    }

    public void ClearFormCache(Form form)
    {
        foreach (var control in EnumerateControls(form))
        {
            control.Tag = null;
        }
    }

    private void ApplyToControl(Control control)
    {
        // 首次遇到：缓存原始中文文本；之后：用缓存文本重新翻译。
        if (control.Tag is not string originalText)
        {
            if (string.IsNullOrEmpty(control.Text))
            {
                return;
            }

            originalText = control.Text;
            control.Tag = originalText;
        }

        control.Text = GetText(originalText);
    }

    private static IEnumerable<Control> EnumerateControls(Control root)
    {
        var queue = new Queue<Control>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return current;

            foreach (Control child in current.Controls)
            {
                queue.Enqueue(child);
            }
        }
    }
}
