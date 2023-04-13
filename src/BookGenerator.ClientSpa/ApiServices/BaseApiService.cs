using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookGenerator.ClientSpa.ApiServices;

/// <summary>
/// Base class to call api services.
/// </summary>
public abstract class BaseApiService
{
    private readonly JsonSerializerOptions jsonOptions;
    private readonly ILogger<BaseApiService> logger;

    /// <summary>
    /// Http client instance.
    /// </summary>
    protected HttpClient httpClient;

    /// <summary>
    /// Creates an instance of <see cref="BaseApiService"/>
    /// </summary>
    /// <param name="jsonOptions">JSON (de)serialization options</param>
    protected BaseApiService(IOptions<JsonOptions> jsonOptions, ILogger<BaseApiService> logger, IHttpClientFactory httpClientFactory, string? httpClientName)
    {
        if (jsonOptions is null || jsonOptions.Value is null)
        {
            throw new ArgumentNullException(nameof(jsonOptions));
        }

        if (httpClientFactory is null)
        {
            throw new ArgumentNullException(nameof(httpClientFactory));
        }

        if (httpClientName != null)
        {
            httpClient = httpClientFactory.CreateClient(httpClientName);
        }
        else
        {
            httpClient = httpClientFactory.CreateClient();
        }

        this.jsonOptions = jsonOptions.Value.JsonSerializerOptions;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Call the microservice by Web API.
    /// </summary>
    /// <typeparam name="T">The type of returning parameter.</typeparam>
    /// <param name="url">The url to the controller inside the microservice.</param>
    /// <returns>T object.</returns>
    protected async Task<T?> GetAsync<T>(string url)
    {
        var response = await GetAsyncInternal(url).ConfigureAwait(false);
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
    protected async Task<TResponse?> PostAsync<TResponse, TContent>(string url, TContent content)
    {
        string jsonContext = JsonSerializer.Serialize(content);
        var stringContent = new StringContent(jsonContext, Encoding.UTF8, "application/json");

        var response = await PostAsyncInternal(url, stringContent).ConfigureAwait(false);
        if (response.Content == null)
        {
            return default;
        }

        var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<TResponse?>(responseString, jsonOptions);
    }

    /// <summary>
    /// Call the microservice by Web API using GET request.
    /// </summary>
    /// <param name="url">The url to the controller inside the microservice.</param>
    /// <example>/cis.svc/Customer/Clusters?Customer=All</example>
    /// <returns>The Http response from the microservice.</returns>
    private async Task<HttpResponseMessage> GetAsyncInternal(string url)
    {
        var response = await httpClient.GetAsync(url).ConfigureAwait(false);
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
        var response = await httpClient.PostAsync(url, content).ConfigureAwait(false);
        await HandleErrors(response);
        return response;
    }

    private async Task HandleErrors(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            try
            {
                var problemDetails = JsonSerializer.Deserialize<ProblemDetails?>(content, jsonOptions);
                if (problemDetails != null)
                {
                    throw new ApiException(problemDetails);
                }
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Error when deserialize problem details: {content}", content);
            }
            response.EnsureSuccessStatusCode();
        }
    }
}
