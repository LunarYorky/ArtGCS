using System.Globalization;
using Microsoft.Data.Sqlite;
using static ArtGCS.ConstantsManager;
using static ArtGCS.ConstantsManager.SQLQueri;

namespace ArtGCS.DAL;

public class DataBase : IDisposable
{
    private SqliteConnection? _connection;
    private readonly string _workDirectory;

    public DataBase(string workDirectory)
    {
        _workDirectory = workDirectory;
    }

    public bool CreateDB()
    {
        var path = Path.Combine(_workDirectory, DbPath);
        if (File.Exists(path))
        {
            Console.WriteLine("Database already created");
            return false;
        }


        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
        try
        {
            File.Create(path).Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        _connection = new SqliteConnection($"Filename={path}");
        _connection.Open();

        using var command = new SqliteCommand(SQLQueries[Create], _connection);
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return true;
    }

    public Dictionary<string, string>? GetFilesList()
    {
        var path = Path.Combine(_workDirectory, DbPath);
        if (!File.Exists(path))
        {
            Console.WriteLine("[ERROR] DB is not exists");
            return null;
        }

        var filesHashes = new Dictionary<string, string>();
        _connection = new SqliteConnection($"Filename={path}");
        _connection.Open();

        using var command = new SqliteCommand(SQLQueries[FilesHashes], _connection);
        using var reader = command.ExecuteReader();

        if (!reader.HasRows) return filesHashes;

        Console.WriteLine(reader.HasRows);

        while (reader.Read())
        {
            Console.WriteLine(reader.GetString(0));
            Console.WriteLine(reader.GetString(1));
            filesHashes.Add(reader.GetString(0), reader.GetString(1));
        }

        return filesHashes;
    }

    public Guid AddFile(string path, string xxHash, DateTime saveTime)
    {
        var guid = Guid.NewGuid();
        using var command = new SqliteCommand(SQLQueries[NewFile], _connection);
        command.Parameters.AddWithValue("$guid", guid);
        command.Parameters.AddWithValue("$path", path);
        command.Parameters.AddWithValue("$xxHash", xxHash);
        command.Parameters.AddWithValue("$saveTime", DataTimeToString(saveTime));
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return guid;
    }

    public bool AddUser(string name, DateTime dateTime)
    {
        var time = DataTimeToString(dateTime);
        using var command = new SqliteCommand(SQLQueries[InsertUser], _connection);
        command.Parameters.AddWithValue("$name", name);
        command.Parameters.AddWithValue("$first_save_time", time);
        command.Parameters.AddWithValue("$last_update_time", time);
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return true;
    }

    public bool TryAddGallery(Uri uri, string owner, string name)
    {
        var time = DataTimeToString(Time.GetCurrentDateTime());
        using var command = new SqliteCommand(SQLQueries[InsertGalley], _connection);
        command.Parameters.AddWithValue("$uri", uri.ToString());
        command.Parameters.AddWithValue("$resource", uri.Host);
        command.Parameters.AddWithValue("$owner", owner);
        command.Parameters.AddWithValue("$nick_name", name);
        command.Parameters.AddWithValue("$first_save_time", time);
        command.Parameters.AddWithValue("$last_update_time", time);
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return true;
    }

    public bool HashExists(string xxHash, out string? guid)
    {
        using var command = new SqliteCommand(SQLQueries[GetFileGuidByXxHash], _connection);
        command.Parameters.AddWithValue("$XxHash", xxHash);

        using var reader = command.ExecuteReader();

        Console.WriteLine(reader.HasRows);

        if (reader.HasRows)
        {
            Console.WriteLine(reader.Read());

            guid = reader.GetString(0);
            return true;
        }

        guid = null;
        return false;
    }

    private static string DataTimeToString(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private static DateTime StringToDataTime(string time)
    {
        return DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}