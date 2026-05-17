using MySqlConnector;

namespace GalacticLauncher.Backend.Infrastructure;

internal class DbSession(string connectionString) : IAsyncDisposable
{
    public MySqlConnection Connection { get; } = new(connectionString);
    public MySqlTransaction? Transaction { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (Transaction is not null)
            await Transaction.DisposeAsync();

        if (Connection is not null)
            await Connection.DisposeAsync();
    }
}
