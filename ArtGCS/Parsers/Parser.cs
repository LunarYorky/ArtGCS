using ArtGCS.DAL.Entities;
using ArtGCS.Networking;
using HtmlAgilityPack;

namespace ArtGCS.Parsers;

public abstract class Parser
{
    protected string Host { get; init; }

    private readonly IParsHandler _parsHandler;

    protected Parser(IParsHandler parsHandler)
    {
        _parsHandler = parsHandler;
    }

    public async Task ParsProfileGalleryAsync(Uri profileUri, string ownerName)
    {
        var doc = WebDownloader.GetHtml(profileUri);
        if (doc == null)
        {
            LogError("Profile not found");
            return;
        }

        if (await _parsHandler.RegisterGalleryProfileAsync(GetProfile(profileUri, doc), ownerName) == false)
            return; //TODO
        
        await UpdateSubmissions(GetSubmissionLinks(doc), profileUri, ownerName);
    }

    private Task UpdateSubmissions(IEnumerable<Uri> uris, Uri sourceGallery, string ownerName)
    {
        return Task.WhenAll(uris
            .Select(uri => _parsHandler.RegisterSubmissionAsync(GetSubmission(uri, sourceGallery), ownerName))
            .ToArray());
    }

    protected abstract GalleryProfile GetProfile(Uri profileUri, HtmlDocument profileDocument);
    protected abstract Submission? GetSubmission(Uri uri, Uri sourceGallery);
    protected abstract List<Uri> GetSubmissionLinks(HtmlDocument profileDocument);

    protected void LogWarning(string message)
    {
        _parsHandler.Logger.WarningLog(
            $"Class: \"{GetType()}\". Settings for \"{Host}\" {message}");
    }

    protected void LogError(string message)
    {
        _parsHandler.Logger.ErrorLog(
            $"Class: \"{GetType()}\". Settings for \"{Host}\" {message}");
    }
}