using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookGenerator.Client.ApiServices;

/// <summary>
/// Base class to call api services.
/// </summary>
public abstract class BaseApiService
{
    private readonly JsonSerializerOptions jsonOptions;

    /// <summary>
    /// Http client instance.
    /// </summary>
    protected HttpClient httpClient;

    /// <summary>
    /// Creates an instance of <see cref="BaseApiService"/>
    /// </summary>
    /// <param name="jsonOptions">JSON (de)serialization options</param>
    protected BaseApiService(IOptions<JsonOptions> jsonOptions)
    {
        if (jsonOptions is null || jsonOptions.Value is null)
        {
            throw new ArgumentNullException(nameof(jsonOptions));
        }

        this.jsonOptions = jsonOptions.Value.JsonSerializerOptions;
    }

    /// <summary>
    /// Call the microservice by Web API.
    /// </summary>
    /// <typeparam name="T">The type of returning parameter.</typeparam>
    /// <param name="url">The url to the controller inside the microservice.</param>
    /// <returns>T object.</returns>
    protected async Task<T> GetAsync<T>(string url)
    {
        var response = await this.GetAsyncInternal(url).ConfigureAwait(false);
        if (response.Content == null)
        {
            return default;
        }

        var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(responseString, jsonOptions);
    }

    /// <summary>
    /// Call the microservice by Web API.
    /// </summary>
    /// <typeparam name="TResponse">The type of returning parameter.</typeparam>
    /// <param name="url">The url to the controller inside the microservice.</param>
    /// <param name="content">The content sending to microservice.</param>
    /// <returns>T object.</returns>
    protected async Task<TResponse> PostAsync<TResponse, TContent>(string url, TContent content)
    {
        string jsonContext = JsonSerializer.Serialize(content);
        var stringContent = new StringContent(jsonContext, Encoding.UTF8, "application/json");

        var response = await this.PostAsyncInternal(url, stringContent).ConfigureAwait(false);
        if (response.Content == null)
        {
            return default;
        }

        var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<TResponse>(responseString, jsonOptions);
    }

    /// <summary>
    /// Call the microservice by Web API using GET request.
    /// </summary>
    /// <param name="url">The url to the controller inside the microservice.</param>
    /// <example>/cis.svc/Customer/Clusters?Customer=All</example>
    /// <returns>The Http response from the microservice.</returns>
    private async Task<HttpResponseMessage> GetAsyncInternal(string url)
    {
        var response = await this.httpClient.GetAsync(url).ConfigureAwait(false);
        await HandleErrors(response);
        return response;
    }

    /// <summary>
    /// Call the microservice by Web API using POST request.
    /// </summary>
    /// <param name="url">The url to the controller inside the microservice.</param>
    /// <example>/cis.svc/Customer/Clusters?Customer=All</example>
    /// <returns>The Http response from the microservice.</returns>
    private async Task<HttpResponseMessage> PostAsyncInternal(string url, HttpContent content)
    {
        var response = await this.httpClient.PostAsync(url, content).ConfigureAwait(false);
        await HandleErrors(response);
        return response;
    }

    private async Task HandleErrors(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, jsonOptions);
            throw new ApiException(problemDetails);
        }
    }
}
