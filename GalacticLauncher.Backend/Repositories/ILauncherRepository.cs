using GalacticLauncher.Core.DbRecords;

namespace GalacticLauncher.Backend.Repositories;

public interface ILauncherRepository
{
    Task<IEnumerable<GameInfo>> GetAllGamesAsync();
    Task<IEnumerable<ImgInfo>> GetImagesByGameIdAsync(ulong gameId);
    Task<IEnumerable<VersionInfo>> GetVersionsByGameIdAsync(ulong gameId);
    Task<VersionInfo?> GetPrimaryVersionAsync(ulong gameId);
    Task<IEnumerable<ExecInfo>> GetExecsByVersionIdAsync(ulong versionId);
    Task AddGameAsync(GameInfo game);
}