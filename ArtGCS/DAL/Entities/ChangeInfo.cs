using System.ComponentModel.DataAnnotations;

namespace ArtGCS.DAL.Entities;

public class ChangeInfo
{
    [Key] public int Id { get; set; }

    [Required] public DateTime ChangeTime { get; set; }
    [Required] public string TableName { get; set; }
    [Required] public string ColumnName { get; set; }
    [Required] public string PrimaryKey { get; set; }
    public string? OldData { get; set; }
}