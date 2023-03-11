namespace ArtGCS.Parsers.Settings;

public class ParserTypeFaSettings : IParserSettings
{
    public string Host { get; init; }
    public string XpathProfileIcon { get; init; }
    public string XpathProfileName { get; init; }
    public string XpathProfileCreationDataTime { get; init; }
    public string XpathProfileStatus { get; init; }
    public string XpathProfileDescription { get; init; }
    public string UriIconAttributeName { get; init; }
    public string XpathSubmissions { get; init; }
    public string XpathSubmissionPublicationTime { get; init; }
    public string XpathSubmissionFileSrc { get; init; }
    public string XpathSubmissionFileSrcAttribute { get; init; }
    public string XpathSubmissionTitle { get; init; }
    public string XpathSubmissionDescription { get; init; }
    public string XpathSubmissionTags { get; init; }
    public string XpathGalleryUri { get; init; }
    public string XpathNextPageButton { get; init; }
}