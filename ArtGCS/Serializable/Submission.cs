using ArtGCS.Enums;

namespace ArtGCS.Serializable;

public class Submission
{
    public Uri? Uri;
    public Uri? ImageUri;
    public string? Title;
    public string? Description;
    public string? Author;
    public DateTime PublicationTime;
    public DateTime SaveTime;
}