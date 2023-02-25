using System.Text.Json.Serialization;

namespace ArtGCS.Serializable.Meta.Sub;

public class FileMetaInfo
{
    [JsonRequired] public DateTime CreationTime { get; set; }
    [JsonRequired] public DateTime LastSaveTime { get; set; }
    [JsonRequired] public string[] LocalPath { get; set; }
    [JsonRequired] public byte[] XxHash64 { get; set; }
    [JsonRequired] public List<int> Links { get; set; }
}