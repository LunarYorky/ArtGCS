using System.Text.Json;
using ArtGCS.Serializable.Meta;
using static ArtGCS.Storage.ConstantsManager;

namespace ArtGCS.Storage;

public class StorageManager
{
    public string WorkDirectory { get; }

    private MetaManager _metaManager = null!; // init in CreateRepository()
    private FileManager _fileManager = null!; // init in CreateRepository()

    private StorageManager(string workDirectory)
    {
        WorkDirectory = workDirectory;
    }

    public static bool TryReadSystemFiles(string workDirectory, out StorageManager? storageManager)
    {
        storageManager = new StorageManager(workDirectory);

        if (FileManager.TryReadSystemFiles(workDirectory, storageManager, out var fileManager) &&
            MetaManager.TryReadSystemFiles(workDirectory, storageManager, out var metaManager))
        {
            storageManager._metaManager = metaManager!;
            storageManager._fileManager = fileManager!;
            if (storageManager.Init())
                return true;
        }

        storageManager = null;
        return false;
    }

    private bool Init()
    {
        var files = new HashSet<string>(Directory.GetFiles(WorkDirectory));

        AddFiles(WorkDirectory);

        void AddFiles(string root)
        {
            foreach (var directory in Directory.GetDirectories(root))
            {
                foreach (var file in Directory.GetFiles(directory))
                {
                    files.Add(file);
                }

                foreach (var dir in Directory.GetDirectories(directory))
                {
                    AddFiles(dir);
                }
            }
        }
        
        foreach (var fileMetaInfo in _fileManager.FilesMap.Files)
        {
            if (files.Remove(Path.Combine(WorkDirectory, Path.Combine(fileMetaInfo.Value.LocalPath))))
                continue;

            throw new Exception("Corrupted meta files"); //TODO
            return false;
        }

        if (files.Count > 0)
        {
            
            throw new Exception("Unregistered files"); // TODO
        }

        return true;
    }

    public static StorageManager CreateRepository(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(Path.Combine(path, UsersDirectory));
        }
        else if (Directory.GetFiles(path).Length > 0 || Directory.GetDirectories(path).Length > 0)
        {
            throw new Exception();
        }

        var time = Time.GetCurrentDateTime();
        var info = Info.CreateDefault(time);
        var filesMap = FilesMap.CreateDefault(time);

        var storageManager = new StorageManager(path);

        storageManager._fileManager =
            new FileManager(path, storageManager, filesMap); // FileManager needs to be initialized first
        storageManager._metaManager = new MetaManager(path, storageManager, info);

        storageManager.SerializeAndSave(0, storageManager._fileManager.FilesMap);

        return storageManager;
    }

    public void SerializeAndSave<T>(int id, T source)
    {
        if (!_fileManager.FilesMap.Files.TryGetValue(id, out var fileMetaInfo))
            throw new Exception();

        var path = Path.Combine(fileMetaInfo.LocalPath);
        path = Path.Combine(WorkDirectory, path);


        Console.WriteLine(path);
        SaveFile(id, JsonSerializer.Serialize(source));
        _fileManager.RegHash(id);
    }

    public int SerializeAndSave<T>(string[] path, T source)
    {
        var id = _fileManager.NewItem(path);
        Console.WriteLine(id);
        _fileManager.FilesMap.Files[id].LocalPath = path;

        SaveFile(id, JsonSerializer.Serialize(source));

        _fileManager.RegHash(id);
        return id;
    }

    private void SaveFile(int id, string value)
    {
        if (!_fileManager.FilesMap.Files.TryGetValue(id, out var fileMetaInfo))
            throw new Exception();

        var path = Path.Combine(fileMetaInfo.LocalPath);
        path = Path.Combine(WorkDirectory, path);

        fileMetaInfo.LastSaveTime = Time.GetCurrentDateTime();

        if (File.Exists(path))
        {
            File.Copy(path, path + ".bak", true);
            File.WriteAllText(path, value);
            File.Delete(path + ".bak");
        }
        else
        {
            File.WriteAllText(path, value);
        }

        if (id != 0)
            _fileManager.RegHash(id);
    }
}


// private readonly JsonSerializer _serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings()
// {
//     MissingMemberHandling = MissingMemberHandling.Error,
//     Error = delegate(object? sender, ErrorEventArgs args)
//     {
//         args.ErrorContext.Handled = true;
//         var currentError = args.ErrorContext.Member;
//         Console.WriteLine(currentError?.GetType());
//     }
// });