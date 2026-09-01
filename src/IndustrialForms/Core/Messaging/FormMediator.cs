namespace IndustrialForms.Core.Messaging;

/// <summary>
/// 轻量窗体中介者：让互相不认识的窗体通过"主题"解耦通信，
/// 避免窗体之间出现直接的引用依赖。
///
/// 典型用法：一个窗体发布主题，任意数量的窗体订阅同一主题。
/// </summary>
public sealed class FormMediator
{
    private readonly Dictionary<string, List<Action<object?>>> _subscribers = new();

    /// <summary>订阅指定主题。</summary>
    public void Subscribe(string topic, Action<object?> handler)
    {
        if (!_subscribers.TryGetValue(topic, out var handlers))
        {
            handlers = new List<Action<object?>>();
            _subscribers[topic] = handlers;
        }

        handlers.Add(handler);
    }

    /// <summary>发布一条消息到指定主题，所有订阅者都会收到。</summary>
    public void Publish(string topic, object? payload = null)
    {
        if (!_subscribers.TryGetValue(topic, out var handlers))
        {
            return;
        }

        foreach (var handler in handlers.ToArray())
        {
            handler(payload);
        }
    }
}

/// <summary>框架预定义的消息主题，便于集中管理、避免魔法字符串。</summary>
public static class MessageTopics
{
    public const string LanguageChanged = "language.changed";
    public const string ThemeChanged = "theme.changed";
    public const string ConnectionChanged = "connection.changed";
}
