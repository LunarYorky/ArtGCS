using ArtGCS.Enums;

namespace ArtGCS.Serializable;

public class Profile
{
    public DateTime SaveTime;
    public string UserName = null!;
    public Resource Resource;
    
    public Uri? Uri;
    public Uri? UserIconUri;
    public UserInfo? UserInfo;
    public Gallery? Gallery;
}