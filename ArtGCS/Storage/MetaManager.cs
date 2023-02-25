using System.Text.Json;
using ArtGCS.Serializable.Meta;
using static ArtGCS.Storage.ConstantsManager;

namespace ArtGCS.Storage;

public class MetaManager
{
    private readonly string _workDirectory;
    private readonly string _infoPath;

    private StorageManager _storageManager;
    public Info Info { get; private set; } = null!; // init in TryReadInfo()

    private MetaManager(string workDirectory, string infoPath, StorageManager storageManager)
    {
        _workDirectory = workDirectory;
        _infoPath = infoPath;
        _storageManager = storageManager;
    }

    public MetaManager(string workDirectory, StorageManager storageManager, Info info)
    {
        _workDirectory = workDirectory;
        var infoLocalPath = Path.Combine(InfoLocalPath);
        _infoPath = Path.Combine(workDirectory, infoLocalPath);
        _storageManager = storageManager;
        Info = info;

        _storageManager.SerializeAndSave(1, info);
    }

    public static bool TryReadSystemFiles(string workDirectory, StorageManager storageManager,
        out MetaManager? newMetaManager)
    {
        var infoPath = Path.Combine(workDirectory, InfoPath);
        if (File.Exists(infoPath))
        {
            newMetaManager = new MetaManager(workDirectory, infoPath, storageManager);
            if (newMetaManager.TryReadInfo())
                return true;
        }

        newMetaManager = null;
        return false;
    }

    private bool TryReadInfo()
    {
        if (!File.Exists(_infoPath)) return false;
        using var stream = File.Open(_infoPath, FileMode.Open);
        if (JsonSerializer.Deserialize(stream, typeof(Info)) is not Info info)
            return false;

        Info = info;
        return true;
    }
}