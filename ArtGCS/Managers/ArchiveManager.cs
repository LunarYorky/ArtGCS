using ArtGCS.DAL;
using ArtGCS.Parsers;

namespace ArtGCS.Managers;

public class ArchiveManager
{
    private readonly IDataBaseController _dataBaseController;
    private readonly IStorageController _storageController;
    private readonly UniversalParser _universalParser;

    public ArchiveManager(string workDirectory)
    {
        _dataBaseController = new DataBaseController(workDirectory);
        _storageController = new StorageController(_dataBaseController, workDirectory);
        _universalParser = new UniversalParser(_dataBaseController, _storageController);

        CreateSystemFolders(workDirectory);
        ValidateLocalFiles();
    }

    private static void CreateSystemFolders(string workDirectory)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(workDirectory, Constants.MainDbPath)) ??
                                  workDirectory);
        Directory.CreateDirectory(
            Path.GetDirectoryName(Path.Combine(workDirectory, Constants.ChangesAuditDbPath)) ?? workDirectory);

        Directory.CreateDirectory(Path.Combine(workDirectory, Constants.UsersDirectory));
        Directory.CreateDirectory(Path.Combine(workDirectory, Constants.FilesWithoutSourceDirectory));
    }

    public void ValidateLocalFiles()
    {
        _storageController.ValidateFiles(_dataBaseController.GetFilesXxHashes());
    }

    public bool TryAddNewUser(string name)
    {
        return _dataBaseController.TryAddNewUser(name);
    }

    public bool TryAddNewGallery(Uri uri, string ownerName)
    {
        return _dataBaseController.TryAddGalleryProfile(uri, ownerName);
    }

    public Task UpdateGalleryAsync(Uri uri, string ownerName)
    {
        if (!_dataBaseController.GalleryExists(uri))
        {
            TryAddNewUser(ownerName);
            if (!TryAddNewGallery(uri, ownerName))
            {
                return Task.CompletedTask; //TODO fail to update
            }
        }


        return _universalParser.UpdateGalleryAsync(uri, ownerName);
    }
}