using IndustrialForms.Core.Logging;
using IndustrialForms.UI;
using Microsoft.Extensions.DependencyInjection;

namespace IndustrialForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        using var services = DependencyInjection.ConfigureServices();

        Logger.Info("IndustrialForms 应用启动");

        Application.Run(services.GetRequiredService<MainForm>());
    }
}
