namespace IndustrialForms.Core.Localization;

/// <summary>
/// 多语言服务接口。以中文为基准 key，运行时映射到目标语言。
/// </summary>
public interface ILanguageService
{
    /// <summary>是否为英文模式（false 表示中文，默认）。</summary>
    bool IsEnglish { get; set; }

    /// <summary>语言切换事件，供窗体订阅以刷新自身文本。</summary>
    event Action? LanguageChanged;

    /// <summary>注册一条中文到英文的映射。</summary>
    void AddMapping(string cnText, string enText);

    /// <summary>根据基准中文文本获取当前语言下的文本。</summary>
    string GetText(string cnText);

    /// <summary>递归刷新指定窗体所有控件的显示文本。</summary>
    void RefreshFormText(Form form);

    /// <summary>清除指定窗体缓存的原始文本（窗体关闭时调用）。</summary>
    void ClearFormCache(Form form);
}
