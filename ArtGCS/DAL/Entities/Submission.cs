using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtGCS.DAL.Entities;

public class Submission
{
    [Key] public Uri Uri { get; set; } = null!;

    public Uri SourceGalleryUri { get; set; } = null!;
    [ForeignKey("SourceGalleryUri")] public GalleryProfile SourceGallery { get; set; } = null!; //  foreign key. Gallery
    public Guid? SubmissionFileGuid { get; set; }
    [ForeignKey("SubmissionFileGuid")] public FileMetaInfo? SubmissionFile { get; set; } //  foreign key. File
    public Uri? SubmissionFileUri { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }

    public DateTime? PublicationTime { get; set; }
    public DateTime FirstSaveTime { get; set; }
    public DateTime LastUpdateTime { get; set; }
}