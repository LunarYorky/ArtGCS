using ArtGCS.DAL.Entities;

namespace ArtGCS.DAL;

public interface IDataBaseController
{
    Dictionary<string, byte[]> GetFilesXxHashes();
    GalleryProfile? GetGalleryProfile(Uri uri);
    Submission? GetSubmission(Uri uri);

    void UpdateGalleryProfile(GalleryProfile galleryProfile);
    void UpdateSubmission(Submission submission);
    FileMetaInfo? FindFileByHash(byte[] xxHash);

    // public bool HashExists(byte[] xxHash, out Guid? guid);
    bool TryAddGalleryProfile(GalleryProfile galleryProfile);
    void AddNewFileMetaInfo(FileMetaInfo fileMetaInfo);
    void AddNewSubmission(Submission submission);
    bool TryAddNewUser(string name);
    bool TryAddGalleryProfile(Uri uri, string ownerName);
    bool GalleryExists(Uri uri);
}