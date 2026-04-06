using GalacticLauncher.Core.DbRecords;

namespace GalacticLauncher.Backend.Repositories;

public interface IExecRepository
{
    Task<IEnumerable<ExecInfo>> GetExecsByVersionId(long versionId);
}
