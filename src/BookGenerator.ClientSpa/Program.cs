using BookGenerator.ClientSpa.ApiServices;
using BookGenerator.ClientSpa.BackgroundWorkers;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Net.Http.Headers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<IBookApiService, BookApiService>()
    .AddHostedService<StartupWorker>();

builder.Services
    .AddHttpClient("BookApiClient", client =>
    {
        string? baseUrl = builder.Configuration["BookApiService:BaseUrl"];
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
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/dist";
});

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseSpaStaticFiles();

app.UseSpa(spa => {
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.Options.DevServerPort = 5173;
        spa.UseReactDevelopmentServer(npmScript: "start");
        
    }
});

app.Run();
