namespace IndustrialForms.Core.Storage;

/// <summary>
/// 通信协议配置模型（通用字段）。
/// 协议的具体参数（串口波特率、数据位、从站地址等）以 JSON 形式存入 Settings，
/// 字段可自由扩展而无需频繁修改表结构。
/// </summary>
public sealed class CommunicationProtocol
{
    /// <summary>数据库自增主键。</summary>
    public long Id { get; set; }

    /// <summary>协议名称，如 "Modbus RTU"。</summary>
    public string Name { get; set; } = "";

    /// <summary>传输方式：Serial / Tcp / Udp。</summary>
    public string Transport { get; set; } = "Serial";

    /// <summary>协议参数 JSON 字符串。</summary>
    public string Settings { get; set; } = "{}";

    /// <summary>描述信息。</summary>
    public string Description { get; set; } = "";

    /// <summary>最后更新时间。</summary>
    public string UpdatedAt { get; set; } = "";
}
