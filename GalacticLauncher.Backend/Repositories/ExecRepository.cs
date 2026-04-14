using Dapper;
using GalacticLauncher.Core.DbRecords;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface IExecRepository
{
    Task<IEnumerable<ExecInfo>> GetExecsByVersionId(long versionId);
}

public class ExecRepository(MySqlConnection db) : IExecRepository
{
    public async Task<IEnumerable<ExecInfo>> GetExecsByVersionId(long versionId)
    {
        return await db.QueryAsync<ExecInfo>(
            "SELECT * FROM execs WHERE id_version = @p1",
            new { p1 = versionId }
        );
    }
}
