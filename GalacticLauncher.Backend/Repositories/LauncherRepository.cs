using GalacticLauncher.Backend.Database;
using GalacticLauncher.Core.DbRecords;
using GalacticLauncher.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GalacticLauncher.Backend.Repositories;

public class LauncherRepository(AppDbContext db) : ILauncherRepository
{
    public async Task<IEnumerable<GameInfo>> GetAllGamesAsync() 
        => await db.Games.ToListAsync();
    
    public async Task<IEnumerable<TagInfo>> GetAllTagsByGameIdAsync(ulong gameId)
    {
        return await (from gt in db.Set<GameTagInfo>()
            join t in db.Tags on gt.IdTag equals t.Id
            where gt.IdGame == gameId
            select t).ToListAsync();
    }

    public async Task<IEnumerable<ImgInfo>> GetImagesByGameIdAsync(ulong gameId) 
        => await db.Images.Where(i => i.IdGame == gameId).ToListAsync();

    public async Task<IEnumerable<VersionInfo>> GetVersionsByGameIdAsync(ulong gameId) 
        => await db.Versions.Where(v => v.IdGame == gameId).ToListAsync();

    public async Task<VersionInfo?> GetPrimaryVersionAsync(ulong gameId) 
        => await db.Versions.FirstOrDefaultAsync(v => v.IdGame == gameId && v.IsPrimary);

    public async Task<IEnumerable<ExecInfo>> GetExecsByVersionIdAsync(ulong versionId) 
        => await db.Execs.Where(e => e.IdVersion == versionId).ToListAsync();

    public async Task AddGameAsync(GameInfo game)
    {
        await db.Games.AddAsync(game);
        await db.SaveChangesAsync(); // Insert into in Sql
    } // Could be used in the future to add games to the database
}