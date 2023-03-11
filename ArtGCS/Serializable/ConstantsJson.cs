namespace ArtGCS.Serializable;

public class ConstantsJson
{
    public string MetaFilesDirectory { get; init; }
    public string MainDBName { get; init; }
    public string UsersDirectoryName { get; init; }
    public string SQLFilesDirectory { get; init; }
    public string LogDirectoryName { get; init; }
    public string FilesWithoutSource { get; init; }
    public string ParsersConfigsDirectory { get; init; }
    public string ChangesAuditDbName { get; set; }
    public string DefaultOtherFolderName { get; set; }
}