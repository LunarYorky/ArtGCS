using System.Globalization;
using ArtGCS.DAL.Entities;
using ArtGCS.Networking;
using ArtGCS.Parsers.Settings;
using HtmlAgilityPack;

namespace ArtGCS.Parsers;

public class ParserTypeFa : Parser
{
    protected readonly ParserTypeFaSettings ParserTypeFaSettings;

    private const string ErrorMsg =
        "It is not right. It shouldn't be like that. NextGalleryPage() should return false if there is no doc.";

    public ParserTypeFa(ParsHandler parsHandler, ParserTypeFaSettings parserTypeFaSettings) : base(parsHandler)
    {
        ParserTypeFaSettings = parserTypeFaSettings;
        Host = ParserTypeFaSettings.Host;
    }

    protected DateTime TimeToDataDateTime(string time)
    {
        time = time[5..];
        Console.WriteLine($"\"{time}\" ParseTypeFa. Time convertion debug");

        if (DateTime.TryParseExact(time, "dd MMM yyyy hh:mm:ss", //Sun, 11 Dec 2022 00:45:38 GMT
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
        {
            return dateTime;
        }

        if (DateTime.TryParseExact(time, "dd MMM yyyy hh:mm:ss",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
        {
            return dateTime;
        }

        LogWarning($"Failed to convert date time. \"{time}\"");
        return DateTime.MinValue;
    }

    protected override GalleryProfile GetProfile(Uri profileUri, HtmlDocument profileDocument)
    {
        return new GalleryProfile
        {
            Uri = profileUri,
            NickName = GetProfileName(profileUri, profileDocument),
            CreationTime = GetProfileCreationTime(profileUri, profileDocument),
            Status = GetProfileStatus(profileDocument),
            Description = GetProfileDescription(profileDocument),
            IconFileUri = GetProfileIconUri(profileDocument),
        };
    }

    protected override Submission? GetSubmission(Uri uri, Uri sourceGallery)
    {
        var submissionDocument = WebDownloader.GetHtml(uri);
        if (submissionDocument != null)
            return new Submission
            {
                Uri = uri,
                SourceGalleryUri = sourceGallery,
                SubmissionFileUri = GetSubmissionFileUri(submissionDocument, uri),
                Title = GetSubmissionTitle(submissionDocument, uri),
                Description = GetSubmissionDescription(submissionDocument, uri),
                Tags = GetSubmissionTags(submissionDocument, uri),
                PublicationTime = GetPublicationTime(submissionDocument, uri)
            };
        LogError($"\"{uri}\" Failed to load submission html doc. Parsing of this page is canceled.");
        return null;
    }

    protected virtual string? GetProfileName(Uri profileUri, HtmlDocument profileDocument)
    {
        var node = profileDocument.DocumentNode.SelectSingleNode(ParserTypeFaSettings.XpathProfileName);
        if (node != null) return node.InnerHtml.Trim();

        LogWarning($"\"{profileUri}\" Profile name not found");
        return null;
    }

    protected virtual DateTime GetProfileCreationTime(Uri profileUri, HtmlDocument profileDocument)
    {
        var node = profileDocument.DocumentNode.SelectSingleNode(ParserTypeFaSettings.XpathProfileCreationDataTime);

        if (node != null)
        {
            var time = node.InnerHtml;
            var index = time.LastIndexOf('>') + 1;
            time = time.Substring(index, time.Length - 1 - index);

            Console.WriteLine($"\"{time}\" 120 ParseTypeFa");

            if (DateTime.TryParseExact(time, "MMM dd, yyyy hh:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var dateTime))
            {
                return dateTime;
            }

            if (DateTime.TryParseExact(time, "MMM d, yyyy hh:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateTime))
            {
                return dateTime;
            }
        }

        LogWarning($"\"{profileUri}\" Profile creation time not found");
        return DateTime.MinValue;
    }

    protected virtual string? GetProfileStatus(HtmlDocument profileDocument)
    {
        var node = profileDocument.DocumentNode.SelectSingleNode(ParserTypeFaSettings.XpathProfileStatus);
        if (node != null)
        {
            var text = node.InnerHtml;
            return text[..text.LastIndexOf('|')];
        }

        LogWarning("Profile status not found");
        return null;
    }

    protected virtual string? GetProfileDescription(HtmlDocument profileDocument)
    {
        var node = profileDocument.DocumentNode.SelectSingleNode(ParserTypeFaSettings.XpathProfileDescription);
        if (node != null) return node.InnerHtml;

        LogWarning("Profile description not found");
        return null;
    }

    protected virtual Uri? GetProfileIconUri(HtmlDocument profileDocument)
    {
        var node = profileDocument.DocumentNode.SelectSingleNode(ParserTypeFaSettings.XpathProfileIcon);

        var uri = node?.Attributes[ParserTypeFaSettings.UriIconAttributeName]?.Value;
        if (uri != null && uri.Length > 1)
            return new Uri("https:" + uri);

        return null;
    }

    protected virtual DateTime GetPublicationTime(HtmlDocument document, Uri uri)
    {
        var node = document.DocumentNode.SelectSingleNode(ParserTypeFaSettings.XpathSubmissionPublicationTime);

        if (DateTime.TryParseExact(node?.Attributes.First().Value, "MMM dd, yyyy hh:mm tt",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
        {
            return dateTime;
        }

        if (DateTime.TryParseExact(node?.Attributes.First().Value, "MMM d, yyyy hh:mm tt",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
        {
            return dateTime;
        }

        Console.WriteLine(node?.Attributes.First().Value);
        LogWarning($"\"{uri}\" publication time not found");
        return DateTime.MinValue;
    }

    protected virtual Uri? GetSubmissionFileUri(HtmlDocument document, Uri uri)
    {
        var node = document.DocumentNode.SelectSingleNode(ParserTypeFaSettings.XpathSubmissionFileSrc);
        if (node != null)
        {
            try
            {
                return new Uri("https:" + node.Attributes[ParserTypeFaSettings.XpathSubmissionFileSrcAttribute].Value);
            }
            catch (Exception e)
            {
                LogWarning(e.ToString());
            }

            return null;
        }

        LogWarning($"\"{uri}\" Submission file src not found");
        return null;
    }

    protected virtual string? GetSubmissionTitle(HtmlDocument document, Uri uri)
    {
        var node = document.DocumentNode.SelectSingleNode(ParserTypeFaSettings.XpathSubmissionTitle);
        if (node != null) return node.InnerText;

        LogWarning($"\"{uri}\" Submission title not found");
        return null;
    }

    protected virtual string? GetSubmissionDescription(HtmlDocument document, Uri uri)
    {
        var node = document.DocumentNode.SelectSingleNode(ParserTypeFaSettings.XpathSubmissionDescription);
        if (node != null) return node.InnerHtml;

        LogWarning($"\"{uri}\" Submission description not found");
        return null;
    }

    protected virtual string? GetSubmissionTags(HtmlDocument document, Uri uri)
    {
        var nodes = document.DocumentNode.SelectNodes(ParserTypeFaSettings.XpathSubmissionTags);
        if (nodes != null)
            return nodes.Aggregate("", (current, node) => current + (node.InnerText + ", "));


        LogWarning($"\"{uri}\" Submission tags not found");
        return null;
    }

    protected override List<Uri> GetSubmissionLinks(HtmlDocument profileDocument)
    {
        var pages = GetGalleryPages(profileDocument);

        var uris = new List<Uri>();
        foreach (var page in pages)
        {
            var nods = page.DocumentNode.SelectNodes(ParserTypeFaSettings.XpathSubmissions);
            if (nods == null)
            {
                LogWarning("Submissions not found on one of the pages");
                continue;
            }

            uris.AddRange(nods.Select(node => new Uri("https://" + Host + node.Attributes.First().Value)));
        }

        return uris;
    }

    protected virtual List<HtmlDocument> GetGalleryPages(HtmlDocument profileDocument)
    {
        var pages = new List<HtmlDocument>();

        var uri = GetGalleryUri(profileDocument);
        if (uri == null) return pages;

        var doc = WebDownloader.GetHtml(uri);
        if (doc == null) return pages;

        pages.Add(doc);

        while (TryGetNextGalleryPage(doc, out doc))
            pages.Add(doc ?? throw new InvalidOperationException(ErrorMsg));

        return pages;
    }

    protected virtual Uri? GetGalleryUri(HtmlDocument profileDocument)
    {
        var node = profileDocument.DocumentNode.SelectSingleNode(ParserTypeFaSettings.XpathGalleryUri);
        if (node != null)
        {
            try
            {
                return new Uri("https://" + Host + node.Attributes.First().Value);
            }
            catch
            {
                // ignored
            }
        }

        LogWarning("Profile gallery not found");
        return null;
    }

    protected virtual bool TryGetNextGalleryPage(HtmlDocument page, out HtmlDocument? nextPage)
    {
        var next = page.DocumentNode.SelectSingleNode(ParserTypeFaSettings.XpathNextPageButton);
        if (next != null && next.InnerText.Contains("Next"))
        {
            nextPage = WebDownloader.GetHtml(new Uri("https://" + Host + next.Attributes.First().Value));
            return true;
        }

        nextPage = null;
        return false;
    }
}