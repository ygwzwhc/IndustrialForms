using IndustrialForms.Core.Localization;
using IndustrialForms.Core.Messaging;
using IndustrialForms.Core.Storage;
using IndustrialForms.UI;
using Microsoft.Extensions.DependencyInjection;

namespace IndustrialForms;

/// <summary>
/// 依赖注入装配中心：集中注册所有框架服务与窗体。
///
/// 分层约定：
/// 1. 基础设施层（多语言、消息中介）—— 单例，全局共享；
/// 2. 窗体 —— 瞬态，由子窗体管理器按需创建并缓存。
/// </summary>
public static class DependencyInjection
{
    public static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // 基础设施
        services.AddSingleton<ILanguageService>(_ => BuildLanguageService());
        services.AddSingleton<FormMediator>();

        // 数据存储（SQLite：参数 + 通信协议持久化）
        services.AddSingleton<AppDatabase>();
        services.AddSingleton<ParameterRepository>();
        services.AddSingleton<ProtocolRepository>();

        // 窗体
        services.AddTransient<MainForm>();
        services.AddTransient<DashboardForm>();
        services.AddTransient<SettingsForm>();
        services.AddTransient<AboutForm>();
        services.AddTransient<LogViewerForm>();

        return services.BuildServiceProvider();
    }

    private static LanguageService BuildLanguageService()
    {
        var service = new LanguageService();

        // 预置通用文本映射（中文 -> 英文）。业务扩展时继续调用 AddMapping 即可。
        service.AddMapping("文件", "File");
        service.AddMapping("视图", "View");
        service.AddMapping("语言", "Language");
        service.AddMapping("帮助", "Help");
        service.AddMapping("退出", "Exit");
        service.AddMapping("仪表盘", "Dashboard");
        service.AddMapping("设置", "Settings");
        service.AddMapping("日志查看器", "Log Viewer");
        service.AddMapping("关于", "About");
        service.AddMapping("当前页面", "Current page");
        service.AddMapping("框架运行概况（示例数据）", "Framework overview (sample data)");
        service.AddMapping("触发一条提示", "Trigger a toast");
        service.AddMapping("操作成功", "Operation succeeded");
        service.AddMapping("工业级 WinForms 上位机 UI 框架", "Industrial WinForms HMI UI Framework");
        service.AddMapping("数据存储", "Data Storage");
        service.AddMapping("数据库文件", "Database file");
        service.AddMapping("参数条目", "Parameters");
        service.AddMapping("通信协议", "Protocols");

        return service;
    }
}
