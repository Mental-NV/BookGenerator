using BookGenerator.Client.ApiServices;
using BookGenerator.Client.BackgroundWorkers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<IBookApiService, BookApiService>()
    .AddHostedService<StartupWorker>();

builder.Services
    .AddHttpClient("BookApiClient", client =>
    {
        string baseUrl = builder.Configuration["BookApiService:BaseUrl"];
        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new ApplicationException("Base url for BookApi is not set");
        }
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    });

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
