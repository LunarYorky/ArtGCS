using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtGCS.DAL.Entities;

public class GalleryProfile
{
    [Key] public Uri Uri { get; set; } = null!;
    public string Resource { get; set; } = null!;

    public User Owner { get; set; } = null!; //  foreign key. User.Name

    public string? NickName { get; set; } = null!;
    public DateTime? CreationTime { get; set; }
    public string? Status { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public Uri? IconFileUri { get; set; } = null!;

    public Guid? IconFileGuid { get; set; } //  foreign key. File.Guid
    
    [ForeignKey("IconFileGuid")] 
    public FileMetaInfo? IconFile { get; set; } = null!; //  foreign key. File.Guid

    public DateTime FirstSaveTime { get; set; }
    public DateTime LastUpdateTime { get; set; }
}