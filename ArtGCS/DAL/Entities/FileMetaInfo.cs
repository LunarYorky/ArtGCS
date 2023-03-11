using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ArtGCS.DAL.Entities;

[Index(nameof(XxHash), Name = "XxHash", IsUnique = true)]
public class FileMetaInfo
{
    [Key] public Guid Guid { get; set; }
    public string LocalPath { get; set; }

    public byte[] XxHash { get; set; }
    public DateTime FirstSaveTime { get; set; }
}