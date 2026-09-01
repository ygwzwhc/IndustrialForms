using System.Collections.Concurrent;
using System.Diagnostics;

namespace IndustrialForms.Core.Logging;

/// <summary>
/// 线程安全的进程内日志中心。
///
/// 设计要点：
/// 1. 静态单例，全局可用，避免向业务代码注入日志依赖；
/// 2. 生产者-消费者解耦：业务线程只负责入队，真正的写入由订阅者（LogFileManager / 日志窗体）完成；
/// 3. 有界队列 + 历史日志截断，防止长期运行内存无限增长；
/// 4. 溯源：自动记录调用方的"类名.方法名"，方便定位日志来源。
/// </summary>
public static class Logger
{
    private const int MaxQueueSize = 10_000;
    private const int MaxHistoryLines = 2_000;

    private static readonly ConcurrentQueue<string> LogQueue = new();
    private static readonly ConcurrentBag<string> HistoryLogs = new();

    private static readonly Dictionary<LogLevel, string> LevelNames = new()
    {
        [LogLevel.Debug] = "DEBUG",
        [LogLevel.Info] = "INFO",
        [LogLevel.Warn] = "WARN",
        [LogLevel.Error] = "ERROR",
    };

    /// <summary>当日志被完整格式化后触发，订阅者负责持久化或展示。</summary>
    public static event Action<string>? LogWritten;

    /// <summary>是否在日志中附带调用方来源信息。</summary>
    public static bool EnableSourceInfo { get; set; } = true;

    /// <summary>当前最低输出级别，低于该级别的日志会被丢弃。</summary>
    public static LogLevel MinimumLevel { get; set; } = LogLevel.Debug;

    static Logger()
    {
        LogWritten += LogFileManager.WriteToFile;
    }

    public static void Debug(string message) => Write(LogLevel.Debug, message);
    public static void Info(string message) => Write(LogLevel.Info, message);
    public static void Warn(string message) => Write(LogLevel.Warn, message);
    public static void Error(string message) => Write(LogLevel.Error, message);
    public static void Error(string message, Exception ex) => Write(LogLevel.Error, $"{message}{Environment.NewLine}异常：{ex}");

    private static void Write(LogLevel level, string message)
    {
        if (level < MinimumLevel)
        {
            return;
        }

        var line = Format(level, message);
        LogQueue.Enqueue(line);
        while (LogQueue.Count > MaxQueueSize && LogQueue.TryDequeue(out _))
        {
        }

        HistoryLogs.Add(line);
        TrimHistory();

        LogWritten?.Invoke(line);
    }

    private static string Format(LogLevel level, string message)
    {
        var source = EnableSourceInfo ? GetSourceInfo() : null;
        return source is null
            ? $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}][{LevelNames[level]}] {message}"
            : $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}][{LevelNames[level]}][{source}] {message}";
    }

    /// <summary>
    /// 从调用栈中提取第一个非 Logger 自身的调用方法，作为日志来源。
    /// </summary>
    private static string? GetSourceInfo()
    {
        var frame = new StackTrace(skipFrames: 2, fNeedFileInfo: false).GetFrames()?
            .FirstOrDefault(f =>
            {
                var method = f.GetMethod();
                return method is not null && method.DeclaringType != typeof(Logger);
            });

        if (frame?.GetMethod() is not { } method)
        {
            return null;
        }

        return $"{method.DeclaringType?.Name}.{method.Name}";
    }

    private static void TrimHistory()
    {
        var snapshot = HistoryLogs.ToArray();
        if (snapshot.Length <= MaxHistoryLines)
        {
            return;
        }

        var keep = snapshot.Skip(snapshot.Length - MaxHistoryLines).ToList();
        while (HistoryLogs.TryTake(out _))
        {
        }

        foreach (var line in keep)
        {
            HistoryLogs.Add(line);
        }
    }

    /// <summary>返回历史日志快照（供日志窗体启动时回填）。</summary>
    public static string[] GetHistoryLogs() => HistoryLogs.ToArray();
}
