using Microsoft.Data.Sqlite;

namespace IndustrialForms.Core.Storage;

/// <summary>
/// SQLite 数据库连接与结构管理。
///
/// 工业上位机中，设备参数、通信协议等配置统一落库保存，避免散落在配置文件或代码里。
/// 选用 SQLite 的原因：单文件、零配置、免安装——部署到现场无需额外搭建数据库环境。
/// </summary>
public sealed class AppDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    /// <summary>数据库文件完整路径。</summary>
    public string DatabasePath { get; }

    public AppDatabase()
    {
        // 数据库文件放在应用目录下的 data 子目录，随程序一起分发。
        var dataDir = Path.Combine(AppContext.BaseDirectory, "data");
        Directory.CreateDirectory(dataDir);
        DatabasePath = Path.Combine(dataDir, "industrialforms.db");

        _connection = new SqliteConnection($"Data Source={DatabasePath}");
        _connection.Open();

        EnsureCreated();
        SeedSampleData();
    }

    /// <summary>底层连接（供各仓库复用，全程单连接、线程安全由调用方控制）。</summary>
    public SqliteConnection Connection => _connection;

    private void EnsureCreated()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Parameters (
                Key         TEXT PRIMARY KEY,
                Value       TEXT NOT NULL,
                Description TEXT NULL,
                UpdatedAt   TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS CommunicationProtocols (
                Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                Name        TEXT NOT NULL,
                Transport   TEXT NOT NULL,
                Settings    TEXT NOT NULL,
                Description TEXT NULL,
                UpdatedAt   TEXT NOT NULL
            );
            """;
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// 首次运行时写入演示数据，用于展示"参数 + 通信协议落库"的完整链路。
    /// 实际项目中可替换为真实的参数导入 / 协议配置。
    /// </summary>
    private void SeedSampleData()
    {
        var parameters = new ParameterRepository(this);
        if (parameters.Count() == 0)
        {
            parameters.Set("MachineName", "Demo-Machine-01", "设备名称");
            parameters.Set("WorkMode", "Auto", "运行模式");
            parameters.Set("MaxSpeed", "1200", "最大转速（rpm）");
            parameters.Set("Language", "zh-CN", "界面语言偏好");
        }

        var protocols = new ProtocolRepository(this);
        if (protocols.Count() == 0)
        {
            protocols.Add(new CommunicationProtocol
            {
                Name = "Modbus RTU",
                Transport = "Serial",
                Settings = """{"port":"COM3","baudRate":115200,"dataBits":8,"stopBits":1,"parity":"None","slaveId":1}""",
                Description = "通用串口通信示例",
            });
        }
    }

    public void Dispose() => _connection.Dispose();
}
