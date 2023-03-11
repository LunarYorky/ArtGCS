using ArtGCS.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtGCS.DAL;

public class DataBaseController : IDataBaseController
{
    private readonly string _workDirectory;

    public DataBaseController(string workDirectory)
    {
        _workDirectory = workDirectory;
    }

    public Dictionary<string, byte[]> GetFilesXxHashes()
    {
        using var context = new MainDbContext(_workDirectory);

        return context.FilesMetaInfo.ToDictionary(metaInfo => metaInfo.LocalPath, metaInfo => metaInfo.XxHash);
    }

    public GalleryProfile? GetGalleryProfile(Uri uri)
    {
        using var context = new MainDbContext(_workDirectory);
        return context.GalleryProfiles.Find(uri);
    }

    public Submission? GetSubmission(Uri uri)
    {
        using var context = new MainDbContext(_workDirectory);
        return context.Submissions.Find(uri);
    }

    public void UpdateGalleryProfile(GalleryProfile galleryProfile)
    {
        galleryProfile.LastUpdateTime = Time.GetCurrentDateTime();

        using var context = new MainDbContext(_workDirectory);

        var localGalleryProfile = context.GalleryProfiles.Find(galleryProfile.Uri);
        context.Entry(localGalleryProfile).CurrentValues.SetValues(galleryProfile);

        context.SaveChanges();
    }

    public void UpdateSubmission(Submission submission)
    {
        submission.LastUpdateTime = Time.GetCurrentDateTime();

        using var context = new MainDbContext(_workDirectory);

        var localSubmission = context.Submissions.Find(submission.Uri);
        context.Entry(localSubmission).CurrentValues.SetValues(submission);

        context.SaveChanges();
    }

    public void AddNewFileMetaInfo(FileMetaInfo fileMetaInfo)
    {
        using var context = new MainDbContext(_workDirectory);
        context.FilesMetaInfo.Add(fileMetaInfo);
        context.SaveChanges();
    }

    public void AddNewSubmission(Submission submission)
    {
        var time = Time.GetCurrentDateTime();
        submission.FirstSaveTime = time;
        submission.LastUpdateTime = time;

        using var context = new MainDbContext(_workDirectory);
        context.Submissions.Add(submission);
        Console.WriteLine(submission.Uri + "SAVE");
        context.SaveChanges();
    }

    public bool TryAddNewUser(string name)
    {
        using var context = new MainDbContext(_workDirectory);
        var time = Time.GetCurrentDateTime();
        context.Users.Add(new User
        {
            Name = name,
            FirstSaveTime = time,
            LastUpdateTime = time
        });

        return TrySaveChanges(context);
    }

    public bool TryAddGalleryProfile(Uri uri, string ownerName)
    {
        using var context = new MainDbContext(_workDirectory);
        var time = Time.GetCurrentDateTime();

        var owner = context.Users.Find(ownerName);
        if (owner == null)
            return false;

        context.GalleryProfiles.Add(new GalleryProfile
        {
            Uri = uri,
            Resource = uri.Host,
            Owner = owner,
            FirstSaveTime = time,
            LastUpdateTime = time
        });

        return TrySaveChanges(context);
    }

    public bool TryAddGalleryProfile(GalleryProfile galleryProfile)
    {
        var time = Time.GetCurrentDateTime();
        galleryProfile.FirstSaveTime = time;
        galleryProfile.LastUpdateTime = time;

        using var context = new MainDbContext(_workDirectory);
        context.GalleryProfiles.Add(galleryProfile);
        return TrySaveChanges(context);
    }

    public bool GalleryExists(Uri uri)
    {
        using var context = new MainDbContext(_workDirectory);
        return context.GalleryProfiles.Any(g => g.Uri == uri);
    }

    public FileMetaInfo? FindFileByHash(byte[] xxHash)
    {
        var context = new MainDbContext(_workDirectory);
        var fileMetaInfos = context.FilesMetaInfo.Where(f => f.XxHash == xxHash);
        return fileMetaInfos.Any() ? fileMetaInfos.First() : null;
    }

    private static bool TrySaveChanges(DbContext dbContext)
    {
        try
        {
            dbContext.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return true;
    }
}