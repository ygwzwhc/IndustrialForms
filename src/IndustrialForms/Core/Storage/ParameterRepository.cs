using Microsoft.Data.Sqlite;

namespace IndustrialForms.Core.Storage;

/// <summary>
/// 键值参数仓库：设备 / 应用运行参数统一以 Key-Value 形式持久化到 SQLite。
/// 提供读写、查询与计数，供业务层按需存取。
/// </summary>
public sealed class ParameterRepository
{
    private readonly AppDatabase _db;

    public ParameterRepository(AppDatabase db) => _db = db;

    /// <summary>按 key 读取参数值，不存在时返回 null。</summary>
    public string? Get(string key)
    {
        using var cmd = _db.Connection.CreateCommand();
        cmd.CommandText = "SELECT Value FROM Parameters WHERE Key = $key";
        cmd.Parameters.AddWithValue("$key", key);
        return cmd.ExecuteScalar() as string;
    }

    /// <summary>写入参数（存在则更新，不存在则插入）。</summary>
    public void Set(string key, string value, string? description = null)
    {
        using var cmd = _db.Connection.CreateCommand();
        cmd.CommandText =
            """
            INSERT INTO Parameters (Key, Value, Description, UpdatedAt)
            VALUES ($key, $value, $desc, $now)
            ON CONFLICT(Key) DO UPDATE SET
                Value       = excluded.Value,
                Description = excluded.Description,
                UpdatedAt   = excluded.UpdatedAt
            """;
        cmd.Parameters.AddWithValue("$key", key);
        cmd.Parameters.AddWithValue("$value", value);
        cmd.Parameters.AddWithValue("$desc", (object?)description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
    }

    /// <summary>读取全部参数（key -> value）。</summary>
    public IReadOnlyDictionary<string, string> GetAll()
    {
        var result = new Dictionary<string, string>();
        using var cmd = _db.Connection.CreateCommand();
        cmd.CommandText = "SELECT Key, Value FROM Parameters ORDER BY Key";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result[reader.GetString(0)] = reader.GetString(1);
        }
        return result;
    }

    /// <summary>参数总数。</summary>
    public int Count()
    {
        using var cmd = _db.Connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Parameters";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
}
