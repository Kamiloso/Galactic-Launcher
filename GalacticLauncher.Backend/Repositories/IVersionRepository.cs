using GalacticLauncher.Core.DbRecords;

namespace GalacticLauncher.Backend.Repositories;

public interface IVersionRepository
{
    Task<IEnumerable<VersionInfo>> GetVersionsByGameId(long gameId);
    Task<VersionInfo?> GetPrimaryVersion(long gameId);
}
