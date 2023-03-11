using ArtGCS.DAL.Entities;

namespace ArtGCS.Parsers;

public interface IParsHandler
{
    Logger Logger { get; }

    Task<bool> RegisterGalleryProfileAsync(GalleryProfile galleryProfile, string? saveFolder);
    Task RegisterSubmissionAsync(Submission? submission, string? saveFolder);
}