using System.IO.Hashing;
using ArtGCS.Networking;

namespace ArtGCS.DAL;

public class StorageManager
{
    private readonly XxHash64 _xxHash64 = new();
    private readonly DataBase _dataBase;
    private HashSet<string> _unregisteredFiles = null!;
    private HashSet<string> _missingFiles = null!;
    private HashSet<string> _changedFiles = null!;

    public StorageManager(DataBase dataBase)
    {
        _dataBase = dataBase;
    }

    public async Task<Guid> SaveFile(string directory, Uri uri)
    {
        var stream = await WebDownloader.GetStreamAsync(uri);

        await _xxHash64.AppendAsync(stream);
        if (_dataBase.HashExists(Convert.ToHexString(_xxHash64.GetCurrentHash()), out var guid))
        {
            _xxHash64.Reset();
            return new Guid(guid!);
        }


        var localPath = uri.AbsoluteUri.Split('/', StringSplitOptions.RemoveEmptyEntries)[^1];
        localPath = Path.Combine(directory, localPath);

        FileStream? fileStream;
        if (File.Exists(localPath))
        {
            string newPath;
            for (var i = 0; i < 10; i++)
            {
                newPath = Path.Combine(Path.GetDirectoryName(localPath) ?? string.Empty,
                    Path.GetFileNameWithoutExtension(localPath) + $"({i:D})" + Path.GetExtension(localPath));

                if (File.Exists(newPath)) continue;

                fileStream = File.Create(newPath);
                await stream.CopyToAsync(fileStream);
                return _dataBase.AddFile(newPath, Convert.ToHexString(_xxHash64.GetHashAndReset()),
                    Time.GetCurrentDateTime());
            }

            throw new Exception("File naming limit: " + localPath);
        }

        fileStream = File.Create(localPath);
        await stream.CopyToAsync(fileStream);
        return _dataBase.AddFile(localPath, Convert.ToHexString(_xxHash64.GetHashAndReset()),
            Time.GetCurrentDateTime());
    }

    private Guid RegisterFile(string path)
    {
        using var stream = File.Open(path, FileMode.Open);
        _xxHash64.Append(stream);
        return _dataBase.AddFile(path, Convert.ToHexString(_xxHash64.GetHashAndReset()), Time.GetCurrentDateTime());
    }


    public void ValidateFiles(string workDirectory, Dictionary<string, string> filesHashes)
    {
        _unregisteredFiles = new HashSet<string>(Directory.GetFiles(workDirectory));
        _missingFiles = new HashSet<string>();
        _changedFiles = new HashSet<string>();

        AddFiles(workDirectory);

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

        _unregisteredFiles.Remove(Path.Combine(workDirectory, ConstantsManager.DbPath));

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

            if (pair.Value != Convert.ToHexString(xxHash64.GetHashAndReset()))
                _changedFiles.Add(pair.Key);
        }
    }
}