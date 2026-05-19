using GalacticLauncher.Core;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Services.Networking;

public interface IHttpService
{
    Task GetAsync(string endpoint);
    Task<TResult> GetAsync<TResult>(string endpoint);
    Task PostAsync<TQuery>(string endpoint, TQuery payload);
    Task<TResult> PostAsync<TQuery, TResult>(string endpoint, TQuery payload);
}

internal class HttpService(HttpClient httpClient) : IHttpService
{
    private record ProblemDetailsDto(string? Title, int? Status, string? Detail);

    private static string BuildUrl(string endpoint)
    {
        return $"{Utils.Address}/{endpoint}";
    }

    public async Task GetAsync(
        string endpoint) =>
        await ExecuteSafelyAsync(async () =>
        {
            string url = BuildUrl(endpoint);
            var response = await httpClient.GetAsync(url);

            await EnsureSuccessOrThrowAsync(response);
        });

    public async Task<TResult> GetAsync<TResult>(
        string endpoint) =>
        await ExecuteSafelyAsync(async () =>
        {
            string url = BuildUrl(endpoint);
            var response = await httpClient.GetAsync(url);

            return await HandleJsonResponseAsync<TResult>(response);
        });

    public async Task PostAsync<TQuery>(
        string endpoint, TQuery payload) =>
        await ExecuteSafelyAsync(async () =>
        {
            string url = BuildUrl(endpoint);
            var response = await httpClient.PostAsJsonAsync(url, payload);

            await EnsureSuccessOrThrowAsync(response);
        });

    public async Task<TResult> PostAsync<TQuery, TResult>(
        string endpoint, TQuery payload) =>
        await ExecuteSafelyAsync(async () =>
        {
            string url = BuildUrl(endpoint);
            var response = await httpClient.PostAsJsonAsync(url, payload);

            return await HandleJsonResponseAsync<TResult>(response);
        });

    private static async Task ExecuteSafelyAsync(Func<Task> action)
    {
        try { await action(); }
        catch (Exception ex) { throw WrapException(ex); }
    }

    private static async Task<T> ExecuteSafelyAsync<T>(Func<Task<T>> action)
    {
        try { return await action(); }
        catch (Exception ex) { throw WrapException(ex); }
    }

    private static ApiException WrapException(Exception ex)
    {
        return ex is ApiException apiException
            ? apiException
            : new ApiException("Error while connecting to the server.", ex);
    }

    private async static Task<T> HandleJsonResponseAsync<T>(HttpResponseMessage response)
    {
        await EnsureSuccessOrThrowAsync(response);

        int statusCode = (int)response.StatusCode;

        try
        {
            return (await response.Content.ReadFromJsonAsync<T>())
                ?? throw new JsonException("Parsing returned null.");
        }
        catch (Exception ex)
        {
            throw new ApiException("Error while parsing JSON response from the server.", statusCode, ex);
        }
    }

    private async static Task EnsureSuccessOrThrowAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        int statusCode = (int)response.StatusCode;
        string? reasonPhrase = response.ReasonPhrase;

        string errorMessage = $"HTTP error: {statusCode} {reasonPhrase}";

        try
        {
            ProblemDetailsDto? problem =
                await response.Content.ReadFromJsonAsync<ProblemDetailsDto>() ?? throw new();

            errorMessage = problem.Detail ?? throw new();
        }
        catch { /* ignore errors */ }

        throw new ApiException(errorMessage, statusCode);
    }
}
