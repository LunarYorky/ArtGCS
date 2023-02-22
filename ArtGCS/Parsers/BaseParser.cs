using ArtGCS.Enums;
using ArtGCS.Networking;
using ArtGCS.Serializable;
using HtmlAgilityPack;

namespace ArtGCS.Parsers;

public abstract class BaseParser
{
    protected abstract Resource Resource { get; }

    public Profile ParsProfile(string userName)
    {
        var uri = GetUserProfileUri(userName);
        var userProfileHtml = HtmlLoader.GetHtml(uri);

        return new Profile
        {
            SaveTime = Time.GetCurrentDateTime(),
            UserName = userName,
            Resource = Resource,
            
            Uri = uri,
            UserIconUri = GetUserIconUri(userProfileHtml),
            UserInfo = GenerateUserInfo(userProfileHtml),
            Gallery = GenerateGallery(userName)
        };
    }

    protected abstract UserInfo GenerateUserInfo(HtmlDocument doc);

    protected abstract Uri? GetUserIconUri(HtmlDocument doc);

    private Gallery GenerateGallery(string userName)
    {
        var uri = GetGalleryUri(userName);
        return new Gallery()
        {
            LustUpdate = Time.GetCurrentDateTime(),
            Uri = uri,
            Submissions = GenerateSubmissionList(GetSubmissionLinks(GetGalleryPages(uri)), userName)
        };
    }

    private List<HtmlDocument> GetGalleryPages(Uri uri)
    {
        var pages = new List<HtmlDocument>();
        var doc = HtmlLoader.GetHtml(uri);
        pages.Add(doc);

        while (TryGetNextGalleryPage(doc, out doc))
            pages.Add(doc ??
                      throw new InvalidOperationException("NextGalleryPage() should return false if there is no doc."));

        return pages;
    }

    protected abstract List<Submission> GenerateSubmissionList(List<Uri> uris, string userName);

    protected abstract List<Uri> GetSubmissionLinks(List<HtmlDocument> pages);

    protected abstract bool TryGetNextGalleryPage(HtmlDocument page, out HtmlDocument? nextPage);

    protected abstract Uri GetUserProfileUri(string userName);

    protected abstract Uri GetGalleryUri(string userName);
}