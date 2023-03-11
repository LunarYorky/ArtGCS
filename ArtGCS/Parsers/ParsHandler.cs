using ArtGCS.DAL;
using ArtGCS.DAL.Entities;
using ArtGCS.Managers;

namespace ArtGCS.Parsers;

public class ParsHandler : IParsHandler
{
    private IDataBaseController _dataBaseController;
    private IStorageController _storageController;

    public Logger Logger { get; }

    public ParsHandler(IDataBaseController dataBaseController, IStorageController storageController)
    {
        _dataBaseController = dataBaseController;
        _storageController = storageController;
        Logger = new Logger(storageController.WorkDirectory, GetType().ToString());
    }

    public ParsHandler(IDataBaseController dataBaseController, IStorageController storageController, Logger logger)
    {
        _dataBaseController = dataBaseController;
        _storageController = storageController;
        Logger = logger;
    }

    public virtual async Task<bool> RegisterGalleryProfileAsync(GalleryProfile galleryProfile, string? saveFolder)
    {
        var localGalleryProfile = _dataBaseController.GetGalleryProfile(galleryProfile.Uri);

        if (galleryProfile.IconFileUri != null)
        {
            var response = await _storageController.CheckOrSaveFileAsync(
                saveFolder ?? Constants.DefaultOtherFolderName,
                galleryProfile.IconFileUri);

            if (localGalleryProfile == null)
            {
                galleryProfile.IconFileGuid = response.Item1.Guid;

                return _dataBaseController.TryAddGalleryProfile(galleryProfile);
            }

            localGalleryProfile.IconFileUri = galleryProfile.IconFileUri;
            localGalleryProfile.IconFileGuid = response.Item1.Guid;
        }
        else if (localGalleryProfile == null)
        {
            return _dataBaseController.TryAddGalleryProfile(galleryProfile);
        }

        if (galleryProfile.NickName != null)
            localGalleryProfile.NickName = galleryProfile.NickName;

        if (galleryProfile.CreationTime != null)
            localGalleryProfile.CreationTime = galleryProfile.CreationTime;

        if (galleryProfile.Status != null)
            localGalleryProfile.Status = galleryProfile.Status;

        if (galleryProfile.Description != null)
            localGalleryProfile.Description = galleryProfile.Description;

        if (galleryProfile.NickName != null)
            localGalleryProfile.NickName = galleryProfile.NickName;

        _dataBaseController.UpdateGalleryProfile(localGalleryProfile);
        return true;
    }

    public virtual async Task RegisterSubmissionAsync(Submission? submission, string? saveFolder)
    {
        if (submission == null)
            return;

        var localSubmission = _dataBaseController.GetSubmission(submission.Uri);

        if (submission.SubmissionFileUri != null)
        {
            var response = await _storageController.CheckOrSaveFileAsync(
                saveFolder ?? Constants.DefaultOtherFolderName,
                submission.SubmissionFileUri);

            if (localSubmission == null)
            {
                submission.SubmissionFileGuid = response.Item1.Guid;
                _dataBaseController.AddNewSubmission(submission);
                return;
            }

            localSubmission.SubmissionFileUri = submission.SubmissionFileUri;
            localSubmission.SubmissionFileGuid = response.Item1.Guid;
        }
        else if (localSubmission == null)
        {
            _dataBaseController.AddNewSubmission(submission);
            return;
        }

        if (submission.Title != null)
            localSubmission.Title = submission.Title;

        if (submission.Description != null)
            localSubmission.Description = submission.Description;

        if (submission.Tags != null)
            localSubmission.Tags = submission.Tags;

        if (submission.PublicationTime != null)
            localSubmission.PublicationTime = submission.PublicationTime;

        _dataBaseController.UpdateSubmission(localSubmission);
    }

    protected virtual void LogWarning(string message)
    {
        Logger.WarningLog($"[{GetType()}] {message}");
    }

    protected virtual void LogError(string message)
    {
        Logger.ErrorLog($"[{GetType()}] {message}");
    }
}