using System.Text.Json;
using ArtGCS.Serializable;

namespace ArtGCS;

public static class Constants
{
    private const string ConstantsDirectory = ".\\Constants";
    private const string ConstantsFilePath = ".\\Constants\\Consts.txt";

    private static ConstantsJson _constants;

    static Constants()
    {
        _constants = ReloadConstants(ConstantsFilePath);
    }

    public static ConstantsJson ReloadConstants(string sourcePath)
    {
        using var stream = File.Open(sourcePath, FileMode.Open);
        return JsonSerializer.Deserialize<ConstantsJson>(stream) ??
               throw new Exception("Missing constants resource file");
    }

    public static string MetaFilesDirectory => _constants.MetaFilesDirectory;
    public static string FilesWithoutSourceDirectory => _constants.FilesWithoutSource;
    public static string UsersDirectory => _constants.UsersDirectoryName;
    public static string DefaultOtherFolderName => _constants.DefaultOtherFolderName;
    public static string LogsDirectory => Path.Combine(MetaFilesDirectory, _constants.LogDirectoryName);
    public static string ParsersConfigs => Path.Combine(ConstantsDirectory, _constants.ParsersConfigsDirectory);
    public static string MainDbPath => Path.Combine(MetaFilesDirectory, _constants.MainDBName);
    public static string ChangesAuditDbPath => Path.Combine(MetaFilesDirectory, _constants.ChangesAuditDbName);
}