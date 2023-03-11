using System.ComponentModel.DataAnnotations;

namespace ArtGCS.DAL.Entities;

public class User
{
    [Key] public string Name { get; set; } = null!;

    [Required] public DateTime? FirstSaveTime { get; set; }
    [Required] public DateTime? LastUpdateTime { get; set; }
}