using ArtGCS.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ArtGCS.DAL;

public class ChangesInterceptor : ISaveChangesInterceptor
{
    private readonly string _dbPath;

    public ChangesInterceptor(string dbPath)
    {
        _dbPath = dbPath;
    }

    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var info = CreateChangeInfo(eventData.Context);

        using var changesAuditContext = new ChangesAuditContext(_dbPath);

        foreach (var changeInfo in info)
            changesAuditContext.Add(changeInfo);

        changesAuditContext.SaveChanges();

        return result;
    }

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var infos = CreateChangeInfo(eventData.Context);

        await using var changesAuditContext = new ChangesAuditContext(_dbPath);

        foreach (var changeInfo in infos)
            changesAuditContext.Add(changeInfo);


        await changesAuditContext.SaveChangesAsync(cancellationToken);

        return result;
    }

    private static IEnumerable<ChangeInfo> CreateChangeInfo(DbContext context)
    {
        context.ChangeTracker.DetectChanges();

        var timeNow = Time.GetCurrentDateTime();
        var changesInfo = new List<ChangeInfo>();
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Modified) continue;

            var primaryKey = entry.Metadata.FindPrimaryKey()?.ToString();
            if (primaryKey == null)
            {
                Console.WriteLine("ERROR Inspector. no primaryKey");
                continue;
            }

            foreach (var property in entry.Properties)
            {
                if (property.IsModified)
                {
                    changesInfo.Add(new ChangeInfo
                    {
                        ChangeTime = timeNow,
                        TableName = entry.Metadata.GetTableName() ?? entry.Metadata.Name,
                        ColumnName = property.Metadata.Name,
                        PrimaryKey = primaryKey,
                        OldData = property.OriginalValue?.ToString()
                    });
                }
            }
        }

        return changesInfo;
    }
}