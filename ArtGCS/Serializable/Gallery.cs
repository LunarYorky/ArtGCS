using ArtGCS.Enums;

namespace ArtGCS.Serializable;

public class Gallery
{
    public DateTime LustUpdate;
    public DateTime LocalCreationTime;
    public Uri? Uri;
    public List<Submission>? Submissions;
}