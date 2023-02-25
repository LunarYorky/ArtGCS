using System.Text.Json.Serialization;
using ArtGCS.Serializable.Meta.Sub;
using ArtGCS.Storage;

namespace ArtGCS.Serializable.Meta;

public class FilesMap
{
    [JsonRequired] public List<IdSpace> FreeIdSpace { get; set; }
    [JsonRequired] public Dictionary<int, FileMetaInfo> Files { get; set; }

    public static FilesMap CreateDefault(DateTime time)
    {
        return new FilesMap
        {
            FreeIdSpace = new List<IdSpace>
            {
                new(2, int.MaxValue)
            },
            Files = new Dictionary<int, FileMetaInfo>
            {
                {
                    0, new FileMetaInfo
                    {
                        CreationTime = time,
                        LastSaveTime = time,
                        LocalPath = ConstantsManager.FileMapLocalPath
                    }
                },
                {
                    1, new FileMetaInfo
                    {
                        CreationTime = time,
                        LastSaveTime = time,
                        LocalPath = ConstantsManager.InfoLocalPath
                    }
                }
            }
        };
    }
}