using System.Globalization;
using ArtGCS.Enums;
using ArtGCS.Networking;
using ArtGCS.Serializable;
using HtmlAgilityPack;

namespace ArtGCS.Parsers;

public class FaParser : BaseParser
{
    protected override Resource Resource => Resource.Furaffinity;

    protected override UserInfo GenerateUserInfo(HtmlDocument doc)
    {
        var about = doc.DocumentNode.SelectSingleNode(".//div[@class='section-body userpage-profile']");

        if (about != null)
        {
            return new UserInfo
            {
                UserinfoHtml = about.InnerHtml
            };
        }

        return new UserInfo();
    }

    protected override Uri? GetUserIconUri(HtmlDocument doc)
    {
        var node = doc.DocumentNode.SelectSingleNode(".//userpage-nav-avatar/a/img");

        var uri = node?.Attributes["src"]?.Value;
        if (uri != null && uri.Length > 1)
            return new Uri("https:" + uri);

        return null;
    }

    protected override List<Submission> GenerateSubmissionList(List<Uri> uris, string userName)
    {
        var submissions = new List<Submission>();

        foreach (var uri in uris)
        {
            var doc = WebDownloader.GetHtml(uri);

            var img = doc.DocumentNode.SelectSingleNode(".//img[@id='submissionImg']");

            var imgSrc = new Uri("https:" + img.Attributes["data-fullview-src"].Value);

            var node = doc.DocumentNode.SelectSingleNode(
                ".//div[@id='columnpage']/div[@class='submission-content']/section");

            var title = node
                .SelectSingleNode(
                    "./div/div[@class='submission-id-container']/div[@class='submission-id-sub-container']/div/h2/p")
                .InnerText;

            var description = node
                .SelectSingleNode("./div[@class='section-body']/div").OuterHtml;

            var author =
                node.SelectSingleNode(
                        "./div/div[@class='submission-id-container']/div[@class='submission-id-sub-container']/a/strong")
                    .InnerText;

            var publicationTimeNode = node
                .SelectSingleNode(
                    "./div/div[@class='submission-id-container']/div[@class='submission-id-sub-container']/strong/span")
                .Attributes.First().Value;

            var enUs = new CultureInfo("en-US");
            DateTime publicationTime;
            if (DateTime.TryParseExact(publicationTimeNode, "MMM dd, yyyy hh:mm tt", enUs, DateTimeStyles.None,
                    out publicationTime))
            {
            }
            else if (DateTime.TryParseExact(publicationTimeNode, "MMM d, yyyy hh:mm tt", enUs, DateTimeStyles.None,
                         out publicationTime))
            {
            }

            var s = new Submission
            {
                Uri = uri,
                ImageUri = imgSrc,
                Title = title,
                Description = description,
                Author = author,
                PublicationTime = publicationTime,
                SaveTime = Time.GetCurrentDateTime()
            };

            submissions.Add(s);
        }

        return submissions;
    }

    protected override List<Uri> GetSubmissionLinks(List<HtmlDocument> pages)
    {
        var uris = new List<Uri>();
        foreach (var page in pages)
        {
            var nods = page.DocumentNode.SelectNodes(".//section[@id='gallery-gallery']/figure");
            if (nods == null)
                continue;

            foreach (var node in nods)
            {
                var n = node.SelectSingleNode(".//a");
                if (n == null)
                    continue;

                uris.Add(new Uri("https://www.furaffinity.net" + n.Attributes.First().Value));
            }
        }

        return uris;
    }

    protected override bool TryGetNextGalleryPage(HtmlDocument page, out HtmlDocument? nextPage)
    {
        var next = page.DocumentNode.SelectSingleNode(
            ".//div[@class='section-body']/div[@class='submission-list']/div[@class='aligncenter']/div[last()]/form");
        if (next != null && next.InnerText.Contains("Next"))
        {
            nextPage = WebDownloader.GetHtml(new Uri("https://www.furaffinity.net" + next.Attributes.First().Value));
            return true;
        }

        nextPage = null;
        return false;
    }

    protected override Uri GetUserProfileUri(string userName)
    {
        return new Uri("https://www.furaffinity.net/user/" + userName);
    }

    protected override Uri GetGalleryUri(string userName)
    {
        return new Uri("https://www.furaffinity.net/gallery/" + userName);
    }
}