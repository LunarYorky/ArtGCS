using ArtGCS.DAL;
using ArtGCS.Managers;

namespace ArtGCS.Parsers;

public sealed class UniversalParser // LordParser
{
    private readonly IDataBaseController _dataBaseController;
    private readonly IStorageController _storageController;
    private readonly Logger _logger;
    private readonly Dictionary<string, Parser> _parsers = new();

    public UniversalParser(IDataBaseController dataBaseController, IStorageController storageController)
    {
        _dataBaseController = dataBaseController;
        _storageController = storageController;
        _logger = new Logger(_storageController.WorkDirectory, "Parsers");
    }

    public Task UpdateGalleryAsync(Uri galleryUri, string ownerName)
    {
        return GetParser(galleryUri).ParsProfileGalleryAsync(galleryUri, ownerName);
    }

    private Parser GetParser(Uri uri)
    {
        if (_parsers.TryGetValue(uri.Host, out var parser))
            return parser;

        parser = ParserFactory.Create(CreateParsingHandler(), uri);
        _parsers.Add(uri.Host, parser);
        return parser;
    }

    ParsHandler CreateParsingHandler()
    {
        return new ParsHandler(_dataBaseController, _storageController, _logger);
    }
}