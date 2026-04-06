using GalacticLauncher.Core.DbRecords;

namespace GalacticLauncher.Backend.Repositories;
public interface ITagRepository
{
    Task<IEnumerable<TagInfo>> GetAllTags();
    Task<IEnumerable<TagInfo>> GetTagsByGameId(long gameId);
}
