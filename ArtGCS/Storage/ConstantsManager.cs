namespace ArtGCS.Storage;

public static class ConstantsManager
{
    public static void Reload(string path)
    {
    }


    //Path
    public static string MetaFilesDirectory { get; } = ".ArtGCS";
    public static string UsersDirectory => Path.Combine(MetaFilesDirectory, "users");

    public static string InfoPath => Path.Combine(MetaFilesDirectory, "info.txt");
    public static string FileMapPath => Path.Combine(MetaFilesDirectory, "map.txt");

    public static string[] InfoLocalPath => new[] { MetaFilesDirectory, "info.txt" };
    public static string[] FileMapLocalPath => new[] { MetaFilesDirectory, "map.txt" };
}