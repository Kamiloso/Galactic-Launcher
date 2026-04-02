using GalacticLauncher.Core.DbRecords;

namespace GalacticLauncher.Core.Interfaces;

public interface ILauncherRepository
{
    Task<IEnumerable<GameInfo>> GetAllGamesAsync();
    Task<IEnumerable<TagInfo>> GetAllTagsByGameIdAsync(ulong gameId);
    Task<IEnumerable<ImgInfo>> GetImagesByGameIdAsync(ulong gameId);
    Task<IEnumerable<VersionInfo>> GetVersionsByGameIdAsync(ulong gameId);
    Task<VersionInfo?> GetPrimaryVersionAsync(ulong gameId);
    Task<IEnumerable<ExecInfo>> GetExecsByVersionIdAsync(ulong versionId);
    Task AddGameAsync(GameInfo game);
}