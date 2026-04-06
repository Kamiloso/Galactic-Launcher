using GalacticLauncher.Core.DbRecords;

namespace GalacticLauncher.Backend.Repositories;

public interface IUserRepository
{
    Task<UserInfo?> GetUserByEmail(string email);
    Task<UserInfo?> GetUserByGoogleKey(string googleKey);
}
