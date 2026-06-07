using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using GalacticLauncher.Frontend.Services.Data;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Tools.Networking;

public interface ITelemetryCollector
{
    Task<bool> TrackGameLaunch(ExecInfo execInfo);
}

internal class TelemetryCollector(
    IPreferenceManager preferenceManager,
    IBackendTalker backendTalker) : ITelemetryCollector
{
    public async Task<bool> TrackGameLaunch(ExecInfo execInfo)
    {
        try
        {
            await backendTalker.TrackGameLaunch(
                guid: preferenceManager.Guid,
                playGame: execInfo.ToPlayGame());

            return true;
        }
        catch (ApiException)
        {
            return false;
        }
    }
}
