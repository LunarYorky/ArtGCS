namespace ArtGCS;

public class Gallery
{
    public Gallery(Uri uri, string owner, string nickName, DateTime firstSaveTime, DateTime lastUpdateTime)
    {
        Uri = uri;
        Owner = owner;
        NickName = nickName;
        FirstSaveTime = firstSaveTime;
        LastUpdateTime = lastUpdateTime;
    }

    private string? _status;
    private string? _description;
    private string? _iconFile;
    public DateTime CreationDataTime { get; set; }
    public Uri Uri { get; set; }
    public string Owner { get; set; }
    public string NickName { get; set; }
    public DateTime FirstSaveTime { get; set; }
    public DateTime LastUpdateTime { get; set; }

    public string? Status
    {
        get => _status ?? "null";
        set => _status = value;
    }

    public string? Description
    {
        get => _description ?? "null";
        set => _description = value;
    }

    public string CreationTimeString
    {
        get => CreationDataTime == DateTime.MinValue ? "null" : Time.DataTimeToString(CreationDataTime);
        set => CreationDataTime = Time.StringToDataTime(value);
    }

    public string? IconFile
    {
        get => _iconFile ?? "0000000000000-0000-0000-000000000000";
        set => _iconFile = value;
    }

    public string FirstSaveTimeString
    {
        get => FirstSaveTime == DateTime.MinValue ? "null" : Time.DataTimeToString(FirstSaveTime);
        set => FirstSaveTime = Time.StringToDataTime(value);
    }

    public string LastUpdateTimeString
    {
        get => LastUpdateTime == DateTime.MinValue ? "null" : Time.DataTimeToString(LastUpdateTime);
        set => LastUpdateTime = Time.StringToDataTime(value);
    }
}