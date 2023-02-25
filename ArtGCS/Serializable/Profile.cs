using ArtGCS.Enums;

namespace ArtGCS.Serializable;

public class Profile
{
    public DateTime SaveTime { get; set; }
    public string UserName { get; set; } = null!;
    public Resource Resource { get; set; }

    public Uri? Uri { get; set; }
    public Uri? UserIconUri { get; set; }
    public UserInfo? UserInfo { get; set; }
    public Gallery? Gallery { get; set; }
}