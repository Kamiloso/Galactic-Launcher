using System.Threading.Tasks;
using System;

namespace GalacticLauncher.Frontend.Services
{
    internal interface IApiService
    {
        Task<T> DownloadJsonAsync<T>(string question);
    }
}
