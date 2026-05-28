namespace GalacticLauncher.Backend.Infrastructure;

public interface ITableRepository
{
    Task LockWrite();
}
