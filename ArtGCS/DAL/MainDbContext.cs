using ArtGCS.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtGCS.DAL;

public sealed class MainDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<GalleryProfile> GalleryProfiles { get; set; } = null!;
    public DbSet<Submission> Submissions { get; set; } = null!;
    public DbSet<FileMetaInfo> FilesMetaInfo { get; set; } = null!;
    private string DbPath { get; }
    private readonly ChangesInterceptor _changesInterceptor;

    public MainDbContext(string workDirectory)
    {
        DbPath = Path.Combine(workDirectory, Constants.MainDbPath);
        _changesInterceptor = new ChangesInterceptor(Path.Combine(workDirectory, Constants.ChangesAuditDbPath));
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .AddInterceptors(_changesInterceptor)
            .UseSqlite($"Data Source={DbPath}");
}