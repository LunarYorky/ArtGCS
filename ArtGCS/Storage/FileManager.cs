using System.IO.Hashing;
using System.Text.Json;
using ArtGCS.Serializable.Meta;
using ArtGCS.Serializable.Meta.Sub;
using static ArtGCS.Storage.ConstantsManager;

namespace ArtGCS.Storage;

public class FileManager
{
    private readonly string _workDirectory;

    private readonly string _fileMapPath;

    private StorageManager _storageManager;
    public FilesMap FilesMap { get; private set; } = null!; // init in TryReadInfo()

    private readonly XxHash64 _xxHash64 = new();

    private FileManager(string workDirectory, string fileMapPath, StorageManager storageManager)
    {
        _workDirectory = workDirectory;
        _fileMapPath = fileMapPath;
        _storageManager = storageManager;
    }

    public FileManager(string workDirectory, StorageManager storageManager, FilesMap filesMap)
    {
        _workDirectory = workDirectory;
        var fileMapLocalPath = Path.Combine(FileMapLocalPath);
        _fileMapPath = Path.Combine(workDirectory, fileMapLocalPath);
        _storageManager = storageManager;
        FilesMap = filesMap;
    }

    public void RegHash(int fileId)
    {
        var fileMeta = FilesMap.Files[fileId];
        var path = Path.Combine(_workDirectory, Path.Combine(fileMeta.LocalPath));
        if (!File.Exists(path)) throw new Exception("File not exist");

        using var fileStream = File.Open(path, FileMode.Open);
        _xxHash64.Append(fileStream);
        fileMeta.XxHash64 = _xxHash64.GetHashAndReset();
    }

    public static bool TryReadSystemFiles(string workDirectory, StorageManager storageManager,
        out FileManager? fileManager)
    {
        var fileMapPath = Path.Combine(workDirectory, FileMapPath);
        if (File.Exists(fileMapPath))
        {
            fileManager = new FileManager(workDirectory, fileMapPath, storageManager);
            if (fileManager.TryReadMap())
                return true;
        }

        fileManager = null;
        return false;
    }

    private bool TryReadMap()
    {
        if (!File.Exists(_fileMapPath)) return false;
        using var stream = File.Open(_fileMapPath, FileMode.Open);
        if (JsonSerializer.Deserialize(stream, typeof(FilesMap)) is not FilesMap filesMap)
            return false;
        
        filesMap.FreeIdSpace.Sort((x, y) => y.Start - x.Start);
        for (var i = 0; i + 1 < filesMap.FreeIdSpace.Count; i++)
        {
            if (filesMap.FreeIdSpace[i].End < filesMap.FreeIdSpace[i + 1].Start) continue;

            throw new Exception(); //TODO
            return false;
        }

        foreach (var file in filesMap.Files)
        {
            foreach (var idSpace in filesMap.FreeIdSpace)
            {
                {
                    if (file.Key > idSpace.Start && file.Key < idSpace.End)

                        throw new Exception();
                }
            }
        }

        FilesMap = filesMap;
        return true;
    }

    public int NewItem(string[] path)
    {
        if (FilesMap.FreeIdSpace.Count < 1)
            throw new Exception("Out of id space.");

        var time = Time.GetCurrentDateTime();
        var id = FilesMap.FreeIdSpace[0].Start;
        if (FilesMap.FreeIdSpace[0].Start == FilesMap.FreeIdSpace[0].End)
        {
            FilesMap.FreeIdSpace.RemoveAt(0);
        }
        else
        {
            FilesMap.FreeIdSpace[0] -= 1;
        }

        var newFile = new FileMetaInfo
        {
            CreationTime = time,
            LocalPath = path
        };

        FilesMap.Files.Add(id, newFile);
        return id;
    }
}