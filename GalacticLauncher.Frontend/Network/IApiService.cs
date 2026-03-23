using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace GalacticLauncher.Frontend.Network
{
    internal interface IApiService
    {
        Task<T> GetJsonAsync<T>(string endpoint, Dictionary<string, string>? parameters = null);
        Task<TOut> PostJsonAsync<TIn, TOut>(string endpoint, TIn payload);
    }
}
