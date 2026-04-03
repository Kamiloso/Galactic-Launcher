using GalacticLauncher.Core.DbRecords;

namespace GalacticLauncher.Backend.Repositories;

public interface IVersionRepository
{
    Task<IEnumerable<VersionInfo>> GetVersionsByGameId(ulong gameId);
    Task<VersionInfo?> GetPrimaryVersion(ulong gameId);
}
