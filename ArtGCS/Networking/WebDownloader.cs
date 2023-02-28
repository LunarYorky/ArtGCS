using System.Net;
using System.Threading.RateLimiting;
using HtmlAgilityPack;

namespace ArtGCS.Networking;

public static class WebDownloader
{
    private static readonly HttpClient Client;

    static WebDownloader()
    {
        var options = new TokenBucketRateLimiterOptions()
        {
            TokenLimit = 1,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 1,
            ReplenishmentPeriod = TimeSpan.FromSeconds(2),
            TokensPerPeriod = 1,
            AutoReplenishment = true
        };
        var handler = new HttpClientHandler { AllowAutoRedirect = false }; // TODO headers
        var delegatingHandler = new HttpClientRateLimitedHandler(new TokenBucketRateLimiter(options), handler);
        Client = new HttpClient(delegatingHandler);
    }

    public static HtmlDocument GetHtml(Uri uri)
    {
        Console.WriteLine(uri.ToString());
        var response = Client.GetAsync(uri).Result;
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception("StatusCode: " + response.StatusCode + ". uri: " + uri); //TODO
        }

        var html = response.Content.ReadAsStringAsync().Result;
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc;
    }

    public static async Task<Stream> GetStreamAsync(Uri uri)
    {
        return await Client.GetStreamAsync(uri);
    }
}