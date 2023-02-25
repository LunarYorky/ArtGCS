using System.Text.Json.Serialization;
using ArtGCS.Serializable.Meta.Sub;

namespace ArtGCS.Serializable.Meta;

public class Info
{
    public DateTime CreationTime { get; set; }
    public DateTime LastChangeTime { get; set; }
    public List<UserSheetInfo> UserFilesInfo { get; set; }

    public static Info CreateDefault(DateTime time)
    {
        return new Info
        {
            CreationTime = time,
            LastChangeTime = time,
            UserFilesInfo = new List<UserSheetInfo>()
        };
    }
}