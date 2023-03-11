using System.Net.Http.Headers;
using ArtGCS.DAL.Entities;

namespace ArtGCS.Managers;

public interface IStorageController
{
    public string WorkDirectory { get; }
    Task<(FileMetaInfo, HttpHeaders)> CheckOrSaveFileAsync(string localDirectoryName, Uri uri);
    void ValidateFiles(Dictionary<string, byte[]> filesHashes);
}