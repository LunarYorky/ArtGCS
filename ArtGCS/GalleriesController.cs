using ArtGCS.DAL;

namespace ArtGCS;

public class GalleriesController
{
    private readonly DataBase _dataBase;
    private readonly StorageManager _storageManager;

    public static GalleriesController? Create(string workDirectory)
    {
        var gc = new GalleriesController(workDirectory);
        Directory.CreateDirectory(ConstantsManager.UsersDirectory);
        Directory.CreateDirectory(ConstantsManager.FilesWithoutSourceDirectory);
        return gc._dataBase.CreateDB() ? gc : null;
    }

    public static GalleriesController? Init(string workDirectory)
    {
        var gc = new GalleriesController(workDirectory);
        var files = gc._dataBase.GetFilesList();
        if (files == null)
            return null;

        gc._storageManager.ValidateFiles(workDirectory, files);
        Directory.CreateDirectory(ConstantsManager.UsersDirectory);
        Directory.CreateDirectory(ConstantsManager.FilesWithoutSourceDirectory);
        return gc;
    }

    private GalleriesController(string workDirectory)
    {
        _dataBase = new DataBase(workDirectory);
        _storageManager = new StorageManager(_dataBase);
    }

    public bool TryAddNewUser(string name)
    {
        return _dataBase.AddUser(name, Time.GetCurrentDateTime());
    }

    public bool TryAddNewGallery(Uri uri, string ownerName, string nickName)
    {
        return _dataBase.TryAddGallery(uri, ownerName, nickName);
    }
}