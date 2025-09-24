using Microsoft.Data.Sqlite;

namespace coding_tracker;

internal class DatabaseManager
{
    internal void CreateTable(string connectionString)
    {
        const string query = @"CREATE TABLE IF NOT EXISTS coding (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            Date TEXT,
            Duration TEXT
        )";
        using SqliteConnection conn = new(connectionString);
        using SqliteCommand cmd = new(query, conn);
        conn.Open();

        cmd.ExecuteNonQuery();
    }
}