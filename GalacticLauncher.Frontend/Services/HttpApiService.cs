using System.Net.Http;
using System.Threading.Tasks;
using GalacticLauncher.Core;
using System;
using System.Text.RegularExpressions;
using System.Net.Http.Json;

namespace GalacticLauncher.Frontend.Services
{
    internal class HttpApiService(HttpClient HttpClient) : IApiService
    {
        public async Task<T> DownloadJsonAsync<T>(string question)
        {
            if (!Regex.IsMatch(question, "^[a-z0-9-]+$"))
            {
                throw new ArgumentException($"Invalid question format.");
            }

            T? result = await HttpClient.GetFromJsonAsync<T>(
                $"{Utils.Address}/{question}"
                );

            return result ?? throw new FormatException("Invalid received json format.");
                
        }
    }
}
