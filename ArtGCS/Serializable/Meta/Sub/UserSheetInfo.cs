using System.Text.Json.Serialization;

namespace ArtGCS.Serializable.Meta.Sub;

public class UserSheetInfo
{
    [JsonRequired] public int FileId { get; set; }
    [JsonRequired] public string Name { get; set; }
    [JsonRequired] public DateTime CreationTime { get; set; }
    [JsonRequired] public DateTime LustUpdateTime { get; set; }
}