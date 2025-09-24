using System.Globalization;
using Microsoft.Data.Sqlite;

namespace habit_tracker;

class Program
{
    static readonly string connectionString = @"Data Source=habit-tracker.db";
    static void Main(string[] args)
    {
        const string query = @"CREATE TABLE IF NOT EXISTS drinking_water (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                Quantity INTEGER NOT NULL
            )";

        using SqliteConnection conn = new(connectionString);
        conn.Open();
        using SqliteCommand cmd = new(query, conn);
        cmd.ExecuteNonQuery();

        GetUserInput();
    }

    static void GetUserInput()
    {
        Console.Clear();
        bool closeApp = false;
        while (!closeApp)
        {
            Console.WriteLine("\n\nMAIN MENU");
            Console.WriteLine("\nWhat would you like to do?");
            Console.WriteLine("\nType 0 to Close Application");
            Console.WriteLine("Type 1 to View All Records.");
            Console.WriteLine("Type 2 to Insert Record.");
            Console.WriteLine("Type 3 to Delete Record.");
            Console.WriteLine("Type 4 to Update Record.");
            Console.WriteLine("-----------------------------------------\n");

            string command = Console.ReadLine();

            switch (command)
            {
                case "0":
                    Console.WriteLine("\nGoodbye!\n");
                    closeApp = true;
                    Environment.Exit(0);
                    break;
                case "1":
                    GetAllRecords();
                    break;
                case "2":
                    Insert();
                    break;
                case "3":
                    Delete();
                    break;
                case "4":
                    Update();
                    break;
                default:
                    Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.\n");
                    break;
            }
        }
    }

    private static void GetAllRecords()
    {
        Console.Clear();
        const string query = "SELECT * FROM drinking_water";

        using SqliteConnection conn = new(connectionString);
        using SqliteCommand cmd = new(query, conn);

        List<DrinkingWater> data = [];

        conn.Open();
        using SqliteDataReader reader = cmd.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
                data.Add(new DrinkingWater
                {
                    Id = reader.GetInt32(0),
                    Date = reader.GetDateTime(1),
                    Quantity = reader.GetInt32(2)
                });
        }
        else
            Console.WriteLine("No rows found");

        Console.WriteLine("--------------------------------------------\n");
        foreach (var record in data)
            Console.WriteLine($"{record.Id} - {record.Date.ToString("dd-mm-yyyy")} - Quantity: {record.Quantity}");
        Console.WriteLine("---------------------------------------------\n");
    }

    private static void Insert()
    {
        string date = GetDateInput();

        int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice (no decimals allowed)\n\n");

        const string query = @"INSERT INTO drinking_water(date, quantity) VALUES(@date, @quantity)";

        using SqliteConnection conn = new(connectionString);
        using SqliteCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@date", date);
        cmd.Parameters.AddWithValue("@quantity", quantity);

        conn.Open();
        cmd.ExecuteNonQuery();
    }

    private static void Delete()
    {
        Console.Clear();
        GetAllRecords();

        int id = GetNumberInput("\n\nPlease type the Id of the record you want to delete or type 0 to return to Main Menu\n\n");

        const string query = "DELETE FROM drinking_water WHERE id = @Id";
        using SqliteConnection conn = new(connectionString);
        using SqliteCommand cmd = new(query, conn);

        cmd.Parameters.AddWithValue("@Id", id);
        int rowCount = cmd.ExecuteNonQuery();

        if (rowCount == 0)
        {
            Console.WriteLine($"\n\nRecord with Id {id} doesn't exist.\n\n");
            Delete();
        }
        Console.WriteLine($"\n\nRecord with Id {id} doesn't exist.\n\n");
        GetUserInput();
    }

    private static void Update()
    {
        Console.Clear();
        GetAllRecords();

        int id = GetNumberInput("\n\nPlease type Id of the record would like to update. Type 0 to return to main menu.\n\n");

        const string query = "SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = @id)";

        using SqliteConnection conn = new(connectionString);
        using SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = query;
        conn.Open();
        cmd.Parameters.AddWithValue("@id", id);
        int checkQuery = Convert.ToInt32(cmd.ExecuteScalar());

        if (checkQuery == 0)
        {
            Console.WriteLine($"\n\nRecord with Id {id} doesn't exist.\n\n");
            Update();
        }

        string date = GetDateInput();
        int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice (no decimals allowed)\n\n");

        cmd.CommandText = "UPDATE drinking_water SET date = @date, quantity = @quantity WHERE Id = @id";
        cmd.Parameters.AddWithValue("@date", date);
        cmd.Parameters.AddWithValue("@quantity", quantity);

        cmd.ExecuteNonQuery();
    }

    private static string GetDateInput()
    {
        Console.WriteLine("\n\nPlease insert the date: (Format: dd-mm-yy). Type 0 to return to main menu.");

        string dateInput = Console.ReadLine();

        if (dateInput == "0") GetUserInput();

        return dateInput;
    }

    private static int GetNumberInput(string message)
    {
        Console.WriteLine(message);

        string numberInput = Console.ReadLine();

        if (numberInput == "0") GetUserInput();

        int finalInput = Convert.ToInt32(numberInput);

        return finalInput;
    }
}

public class DrinkingWater
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int Quantity { get; set; }
}
