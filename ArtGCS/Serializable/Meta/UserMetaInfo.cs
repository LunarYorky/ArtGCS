using ArtGCS.Enums;
using ArtGCS.Serializable.Meta.Sub;

namespace ArtGCS.Serializable.Meta;

public class UserMetaInfo
{
    public Dictionary<Resource, Profile> Profiles { get; set; }
}