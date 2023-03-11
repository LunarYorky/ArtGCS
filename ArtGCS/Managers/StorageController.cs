using System.IO.Hashing;
using System.Net.Http.Headers;
using ArtGCS.DAL;
using ArtGCS.DAL.Entities;
using ArtGCS.Networking;

namespace ArtGCS.Managers;

public class StorageController : IStorageController
{
    private const int FileNameLimit = 4;

    private readonly IDataBaseController _dataBaseController;

    private HashSet<string> _unregisteredFiles = null!;
    private HashSet<string> _missingFiles = null!;
    private HashSet<string> _changedFiles = null!;

    public StorageController(IDataBaseController dataBaseController, string workDirectory)
    {
        _dataBaseController = dataBaseController;
        WorkDirectory = workDirectory;
    }

    public string WorkDirectory { get; }

    public async Task<(FileMetaInfo, HttpHeaders)> CheckOrSaveFileAsync(string localDirectoryName, Uri uri)
    {
        var xxHash64 = new XxHash64();

        var responseMessage = await WebDownloader.GetAsync(uri);
        await using var stream = await responseMessage.Content.ReadAsStreamAsync();
        stream.Position = 0;

        await xxHash64.AppendAsync(stream);
        stream.Position = 0;

        var fileMetaInfo = _dataBaseController.FindFileByHash(xxHash64.GetCurrentHash());
        if (fileMetaInfo != null)
            return (fileMetaInfo, responseMessage.Headers);

        var localPath = uri.AbsoluteUri.Split('/', StringSplitOptions.RemoveEmptyEntries)[^1];
        localPath = Path.Combine(WorkDirectory, Constants.UsersDirectory, localDirectoryName, localPath);

        localPath = await SaveFileAsync(stream, localPath);

        fileMetaInfo = new FileMetaInfo
        {
            Guid = Guid.NewGuid(),
            LocalPath = localPath,
            XxHash = xxHash64.GetCurrentHash(),
            FirstSaveTime = Time.GetCurrentDateTime()
        };

        _dataBaseController.AddNewFileMetaInfo(fileMetaInfo);
        return (fileMetaInfo, responseMessage.Headers);
    }

    private async Task<string> SaveFileAsync(Stream sourceStream, string path)
    {
        path = GetFreeName(path);
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);

        await using var localFileStream = File.Create(path);
        await sourceStream.CopyToAsync(localFileStream);
        return path;
    }

    private string GetFreeName(string startPath)
    {
        var newPath = startPath;
        for (var i = 1; File.Exists(newPath) && i < FileNameLimit; i++)
        {
            newPath = Path.Combine(Path.GetDirectoryName(startPath) ?? string.Empty,
                Path.GetFileNameWithoutExtension(startPath) + $"({i:D})" + Path.GetExtension(startPath));
        }

        if (!File.Exists(newPath))
            return newPath;

        throw new Exception("File naming limit: " + startPath); //TODO handle this
    }

    public void ValidateFiles(Dictionary<string, byte[]> filesHashes)
    {
        _unregisteredFiles = new HashSet<string>(Directory.GetFiles(WorkDirectory));
        _missingFiles = new HashSet<string>();
        _changedFiles = new HashSet<string>();

        AddFiles(WorkDirectory);

        void AddFiles(string root)
        {
            foreach (var directory in Directory.GetDirectories(root))
            {
                foreach (var file in Directory.GetFiles(directory))
                {
                    _unregisteredFiles.Add(file);
                }

                foreach (var dir in Directory.GetDirectories(directory))
                {
                    AddFiles(dir);
                }
            }
        }

        _unregisteredFiles.Remove(Path.Combine(WorkDirectory, Constants.MainDbPath));

        var xxHash64 = new XxHash64();
        foreach (var pair in filesHashes)
        {
            if (!_unregisteredFiles.Remove(pair.Key))
            {
                _missingFiles.Add(pair.Key);
                continue;
            }

            using var stream = File.Open(pair.Key, FileMode.Open);
            xxHash64.Append(stream);

            if (xxHash64.GetHashAndReset().SequenceEqual(pair.Value))
                _changedFiles.Add(pair.Key);
        }
    }
}