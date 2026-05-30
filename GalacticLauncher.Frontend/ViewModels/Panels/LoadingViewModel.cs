using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GalacticLauncher.Frontend.ViewModels;

internal partial class LoadingViewModel : ObservableObject
{
    private const int PERIOD_MS = 1000;

    [ObservableProperty]
    private string _loadingText = "";

    private int _step = 0;

    public LoadingViewModel()
    {
        _ = SpinInfinitely();
    }

    private async Task SpinInfinitely()
    {
        while (true)
        {
            _step = (_step % 3) + 1;
            LoadingText = new string('.', _step);

            await Task.Delay(PERIOD_MS);
        }
    }
}
