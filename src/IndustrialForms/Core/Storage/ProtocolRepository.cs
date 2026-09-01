using Microsoft.Data.Sqlite;

namespace IndustrialForms.Core.Storage;

/// <summary>
/// 通信协议仓库：协议的增删改查。
/// 协议参数以 JSON 保存（见 <see cref="CommunicationProtocol.Settings"/>），
/// 新增字段只需扩展 JSON，无需变更表结构。
/// </summary>
public sealed class ProtocolRepository
{
    private readonly AppDatabase _db;

    public ProtocolRepository(AppDatabase db) => _db = db;

    /// <summary>读取全部协议。</summary>
    public IReadOnlyList<CommunicationProtocol> GetAll()
    {
        var list = new List<CommunicationProtocol>();
        using var cmd = _db.Connection.CreateCommand();
        cmd.CommandText =
            """
            SELECT Id, Name, Transport, Settings, Description, UpdatedAt
            FROM CommunicationProtocols
            ORDER BY Id
            """;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new CommunicationProtocol
            {
                Id = reader.GetInt64(0),
                Name = reader.GetString(1),
                Transport = reader.GetString(2),
                Settings = reader.GetString(3),
                Description = reader.IsDBNull(4) ? "" : reader.GetString(4),
                UpdatedAt = reader.GetString(5),
            });
        }
        return list;
    }

    /// <summary>新增协议，返回自增主键。</summary>
    public long Add(CommunicationProtocol protocol)
    {
        using var cmd = _db.Connection.CreateCommand();
        cmd.CommandText =
            """
            INSERT INTO CommunicationProtocols (Name, Transport, Settings, Description, UpdatedAt)
            VALUES ($name, $transport, $settings, $desc, $now);
            SELECT last_insert_rowid();
            """;
        cmd.Parameters.AddWithValue("$name", protocol.Name);
        cmd.Parameters.AddWithValue("$transport", protocol.Transport);
        cmd.Parameters.AddWithValue("$settings", protocol.Settings);
        cmd.Parameters.AddWithValue("$desc", (object?)protocol.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        return (long)cmd.ExecuteScalar()!;
    }

    /// <summary>更新某协议的参数 JSON。</summary>
    public void Update(long id, string settings)
    {
        using var cmd = _db.Connection.CreateCommand();
        cmd.CommandText = "UPDATE CommunicationProtocols SET Settings = $settings, UpdatedAt = $now WHERE Id = $id";
        cmd.Parameters.AddWithValue("$settings", settings);
        cmd.Parameters.AddWithValue("$now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    /// <summary>协议总数。</summary>
    public int Count()
    {
        using var cmd = _db.Connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM CommunicationProtocols";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
}
