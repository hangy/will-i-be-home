using System.Net;
using Microsoft.Extensions.ML;
using WillIBeHome.Api;
using WillIBeHome.ML;
using WillIBeHome.Owntracks;
using WillIBeHome.Shared;

var builder = WebApplication.CreateBuilder(args);

OwntracksSettings? owntracksSettings = builder.Configuration.GetSection("Owntracks").Get<OwntracksSettings>();
builder.Services.AddHttpClient<IOwntracksApiClient, OwntracksApiClient>(c =>
{
    c.BaseAddress = owntracksSettings.Uri;
}).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler { Credentials = new NetworkCredential(owntracksSettings.HttpUserName, owntracksSettings.HttpPassword) });

MLSettings? mlSettings = builder.Configuration.GetSection("ML").Get<MLSettings>();
builder.Services.AddPredictionEnginePool<Transition, WillBeHomePrediction>()
    .FromFile(mlSettings.ModelPath);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapGet("/prediction", async (string user, string device, IOwntracksApiClient owntracksApiClient, PredictionEnginePool<Transition, WillBeHomePrediction> predictionEnginePool, CancellationToken cancellationToken) =>
{
    GetLocationsResult? locations = await owntracksApiClient.GetLocationsAsync(user, device, DateTimeOffset.UtcNow.AddDays(-1), cancellationToken: cancellationToken).ConfigureAwait(false);
    if (locations == null)
    {
        return false;
    }

    IEnumerable<Transition>? transitions = LocationsToTransitionsConverter.Convert(locations.Data.Select(l => LocationConverter.Convert(user, device, l)));
    Transition? lastTransition = transitions.LastOrDefault();
    if (lastTransition == null)
    {
        return false;
    }

    WillBeHomePrediction? predictionResult = predictionEnginePool.Predict(lastTransition);
    return predictionResult.Label;
})
.WithName("GetPrediction");

app.Run();
