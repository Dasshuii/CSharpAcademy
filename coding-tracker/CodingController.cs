using System.Configuration;
using Microsoft.Data.Sqlite;

namespace coding_tracker;

internal class CodingController
{
    readonly string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

    internal void Post(Coding coding)
    {
        const string query = "INSERT INTO coding (date, duration) VALUES (@date, @duration)";
        using SqliteConnection conn = new(connectionString);
        using SqliteCommand cmd = new(query, conn);

        cmd.Parameters.AddWithValue("@date", coding.Date);
        cmd.Parameters.AddWithValue("@duration", coding.Duration);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    internal void Delete(int id)
    {
        string query = "DELETE FROM coding WHERE id = @id";
        using SqliteConnection conn = new(connectionString);
        using SqliteCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();

        Console.WriteLine($"\n\nRecord with Id {id} was deleted.\n\n");
    }

    internal void Update(Coding coding)
    {
        string query = "UPDATE coding SET date = @date, duration = @duration WHERE id = @id";
        using SqliteConnection conn = new(connectionString);
        using SqliteCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@date", coding.Date);
        cmd.Parameters.AddWithValue("@duration", coding.Duration);
        cmd.Parameters.AddWithValue("@id", coding.Id);
        cmd.ExecuteNonQuery();

        Console.WriteLine($"\n\nRecord with Id {id} was updated.\n\n");
    }

    internal void Get()
    {
        List<Coding> data = [];

        string query = "SELECT * FROM coding";
        using SqliteConnection conn = new(connectionString);
        using SqliteCommand cmd = new(query, conn);

        conn.Open();
        using SqliteDataReader reader = cmd.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                data.Add(
                    new Coding
                    {
                        Id = reader.GetInt32(0),
                        Date = reader.GetString(1),
                        Duration = reader.GetString(2)
                    }
                );
            }
        }
        else
        {
            Console.WriteLine("\n\nNo rows found.\n\n");
        }
        Console.WriteLine("\n\n");
        TableVisualisation.ShowTable(data);
    }

    internal Coding GetById(int id)
    {
        const string query = "SELECT * FROM coding WHERE id = @id";
        using SqliteConnection conn = new(connectionString);
        using SqliteCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        conn.Open();

        using SqliteDataReader reader = cmd.ExecuteReader();

        if (reader.HasRows)
        {
            reader.Read();
            return new Coding
            {
                Id = reader.GetInt32(0),
                Date = reader.GetString(1),
                Duration = reader.GetString(2)
            };
        }
        return null;
    }
}