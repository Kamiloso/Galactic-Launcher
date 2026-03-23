using System.Net.Http;
using System.Threading.Tasks;
using GalacticLauncher.Core;
using System;
using System.Text.RegularExpressions;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Linq;

namespace GalacticLauncher.Frontend.Network
{
    internal class HttpApiService(HttpClient HttpClient) : IApiService
    {
        private static void EnsureEndpoint(string endpoint)
        {
            if (!Regex.IsMatch(endpoint, "^[a-z0-9-]+$"))
                throw new ArgumentException($"Invalid endpoint format: {endpoint}");
        }

        public async Task<T> GetJsonAsync<T>(string endpoint, Dictionary<string, string>? parameters = null)
        {
            EnsureEndpoint(endpoint);

            var query = string.Join("&", parameters?.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}") ?? []);

            string postfix = query != string.Empty
                ? "?" + query
                : string.Empty;

            string fullUrl = $"{Utils.Address}/{endpoint}" + postfix;

            T? result = await HttpClient.GetFromJsonAsync<T>(fullUrl);
            return result ?? throw new FormatException("Invalid received json format.");
        }

        public async Task<TOut> PostJsonAsync<TIn, TOut>(string endpoint, TIn payload)
        {
            EnsureEndpoint(endpoint);

            HttpResponseMessage response = await HttpClient.PostAsJsonAsync($"{Utils.Address}/{endpoint}", payload);
            response.EnsureSuccessStatusCode();

            TOut? result = await response.Content.ReadFromJsonAsync<TOut>();
            return result ?? throw new FormatException("Invalid received json format.");
        }
    }
}
