using System.Text;

namespace IndustrialForms.Core.Logging;

/// <summary>
/// 日志文件管理器：按天滚动写入磁盘，并自动清理过期日志。
///
/// 通过订阅 <see cref="Logger.LogWritten"/> 事件工作，自身不关心日志从哪来，
/// 也不关心谁在消费日志，保持单一职责。
/// </summary>
public static class LogFileManager
{
    private static readonly object FileLock = new();
    private static string? _currentFilePath;
    private static DateTime _currentFileDate;

    /// <summary>日志目录（可配置，默认在应用目录下的 Logs 文件夹）。</summary>
    public static string LogDirectory { get; set; } =
        Path.Combine(AppContext.BaseDirectory, "Logs");

    /// <summary>日志文件保留天数，超过则自动删除。</summary>
    public static int RetentionDays { get; set; } = 7;

    public static void WriteToFile(string line)
    {
        try
        {
            lock (FileLock)
            {
                var filePath = GetCurrentFilePath();
                File.AppendAllText(filePath, line + Environment.NewLine, Encoding.UTF8);
            }
        }
        catch
        {
            // 文件写入失败不应影响主流程，静默忽略即可。
        }
    }

    private static string GetCurrentFilePath()
    {
        var today = DateTime.Today;
        if (_currentFilePath is null || _currentFileDate != today)
        {
            Directory.CreateDirectory(LogDirectory);
            _currentFileDate = today;
            _currentFilePath = Path.Combine(LogDirectory, $"app-{today:yyyy-MM-dd}.log");
            CleanExpiredLogs();
        }

        return _currentFilePath;
    }

    private static void CleanExpiredLogs()
    {
        var cutoff = DateTime.Today.AddDays(-RetentionDays);
        foreach (var file in Directory.GetFiles(LogDirectory, "app-*.log"))
        {
            if (File.GetLastWriteTime(file) < cutoff)
            {
                File.Delete(file);
            }
        }
    }
}
