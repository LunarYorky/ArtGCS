using System.Text.Json;
using ArtGCS.Serializable;

namespace ArtGCS;

public static class ConstantsManager
{
    private const string ConstantsDirectory = ".\\Constants";
    private const string ConstantsFilePath = ".\\Constants\\Consts.txt";

    private static ConstantsJson _constants;

    public static readonly Dictionary<SQLQueri, string> SQLQueries = new()
    {
        { SQLQueri.Create, "Create.sql" },
        { SQLQueri.FilesHashes, "FilesHashes.sql" },
        { SQLQueri.NewFile, "INSERT_File.sql" },
        { SQLQueri.GetFileGuidByXxHash, "GetFileGuidByXxHash.sql" },
        { SQLQueri.InsertUser, "INSERT_User.sql" }
    };

    public enum SQLQueri
    {
        Create,
        FilesHashes,
        NewFile,
        GetFileGuidByXxHash,
        InsertUser
    }

    static ConstantsManager()
    {
        _constants = ReloadConstants(ConstantsFilePath);
        LoadSQL();
    }

    public static ConstantsJson ReloadConstants(string sourcePath)
    {
        using var stream = File.Open(sourcePath, FileMode.Open);
        return JsonSerializer.Deserialize<ConstantsJson>(stream) ??
               throw new Exception("Missing constants resource file");
    }

    private static void LoadSQL()
    {
        foreach (var query in SQLQueries)
        {
            try
            {
                SQLQueries[query.Key] = File.ReadAllText(Path.Combine(SQLFilesDirectory, query.Value));
            }
            catch
            {
                throw new Exception("Missing .sql file");
            }
        }
    }

    //Path
    public static string MetaFilesDirectory => _constants.MetaFilesDirectory;
    public static string FilesWithoutSourceDirectory => _constants.filesWithoutSource;
    public static string LogsDirectory => _constants.LogDirectoryName;
    public static string UsersDirectory => Path.Combine(MetaFilesDirectory, _constants.UsersDirectoryName);
    private static string SQLFilesDirectory => Path.Combine(ConstantsDirectory, _constants.SQLFilesDirectory);
    public static string DbPath => Path.Combine(MetaFilesDirectory, _constants.DBName);
}