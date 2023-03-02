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
        reader.Read();
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
        command.Parameters.AddWithValue("$saveTime", Time.DataTimeToString(saveTime));
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
        var time = Time.DataTimeToString(dateTime);
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

    public bool TryAddGallery(Gallery gallery)
    {
        using var command = new SqliteCommand(SQLQueries[InsertGalley], _connection);
        command.Parameters.AddWithValue("$uri", gallery.Uri.ToString());
        command.Parameters.AddWithValue("$resource", gallery.Uri.Host);
        command.Parameters.AddWithValue("$owner", gallery.Owner);
        command.Parameters.AddWithValue("$nick_name", gallery.NickName);
        command.Parameters.AddWithValue("$creation_time", gallery.CreationTimeString);
        command.Parameters.AddWithValue("$status", gallery.Status);
        command.Parameters.AddWithValue("$description", gallery.Description);
        command.Parameters.AddWithValue("$icon_file", gallery.IconFile);
        command.Parameters.AddWithValue("$first_save_time", gallery.FirstSaveTimeString);
        command.Parameters.AddWithValue("$last_update_time", gallery.LastUpdateTimeString);

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

    public bool TryAddSubmission(Submission submission)
    {
        using var command = new SqliteCommand(SQLQueries[InsertSubmission], _connection);
        command.Parameters.AddWithValue("$uri", submission.Uri.ToString());
        command.Parameters.AddWithValue("$source_gallery", submission.SourceGallery);
        command.Parameters.AddWithValue("$file", submission.File);
        command.Parameters.AddWithValue("$file_uri", submission.FileUri.ToString());
        command.Parameters.AddWithValue("$title", submission.Title);
        command.Parameters.AddWithValue("$description", submission.Description);
        command.Parameters.AddWithValue("$tags", submission.Tags);
        command.Parameters.AddWithValue("$publication_time", submission.PublicationTimeString);
        command.Parameters.AddWithValue("$first_save_time", submission.FirstSaveTimeString);
        command.Parameters.AddWithValue("$last_update_time", submission.LastUpdateTimeString);
        command.Parameters.AddWithValue("$last_modified_submission_file",
            submission.LastModifiedSubmissionFileTimeString);

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

    public void Dispose()
    {
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}