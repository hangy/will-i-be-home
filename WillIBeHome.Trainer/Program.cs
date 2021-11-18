using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System.Net;
using WillIBeHome.ML;
using WillIBeHome.Owntracks;
using WillIBeHome.Shared;

namespace WillIBeHome.Trainer;

internal static class Program
{
    internal static async Task Main(string[] args)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        IConfigurationRoot? configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets(typeof(Program).Assembly)
                .Build();

        OwntracksSettings? owntracksSettings = configuration.GetSection("Owntracks").Get<OwntracksSettings>();
        IEnumerable<ML.Location>? locations = await QueryDataAsync(owntracksSettings).ConfigureAwait(false);

        var mlContext = new MLContext(seed: 1);

        IDataView? dataView = mlContext.Data.LoadFromEnumerable(LocationsToTransitionsConverter.Convert(locations));
        DataOperationsCatalog.TrainTestData trainTestData = mlContext.Data.TrainTestSplit(dataView);

        EstimatorChain<Microsoft.ML.Calibrators.CalibratorTransformer<Microsoft.ML.Calibrators.PlattCalibrator>>? pipeline = mlContext.Transforms.Categorical.OneHotEncoding(new[]
        {
                new InputOutputColumnPair(nameof(Transition.User), nameof(Transition.User)),
                new InputOutputColumnPair(nameof(Transition.Device), nameof(Transition.Device)),
                new InputOutputColumnPair(nameof(Transition.FromDayOfWeek), nameof(Transition.FromDayOfWeek)),
                new InputOutputColumnPair(nameof(Transition.FromHours), nameof(Transition.FromHours)),
                new InputOutputColumnPair(nameof(Transition.FromGeohash), nameof(Transition.FromGeohash)),
                new InputOutputColumnPair(nameof(Transition.ToDayOfWeek), nameof(Transition.ToDayOfWeek)),
                new InputOutputColumnPair(nameof(Transition.ToHours), nameof(Transition.ToHours)),
                new InputOutputColumnPair(nameof(Transition.ToGeohash), nameof(Transition.ToGeohash))
            }, OneHotEncodingEstimator.OutputKind.Binary)
        .Append(mlContext.Transforms.Concatenate("Features",
            nameof(Transition.User), nameof(Transition.Device),
            nameof(Transition.FromDayOfWeek), nameof(Transition.FromHours), nameof(Transition.FromGeohash),
            nameof(Transition.ToDayOfWeek), nameof(Transition.ToHours), nameof(Transition.ToGeohash)))
        .Append(mlContext.BinaryClassification.Trainers.LinearSvm())
        .Append(mlContext.BinaryClassification.Calibrators.Platt());

        Console.WriteLine("Training model...");
        TransformerChain<Microsoft.ML.Calibrators.CalibratorTransformer<Microsoft.ML.Calibrators.PlattCalibrator>>? model = pipeline.Fit(trainTestData.TrainSet);

        Console.WriteLine("Predicting...");

        // Now that the model is trained, we want to test it's prediction results, which is done by using a test dataset
        IDataView? predictions = model.Transform(trainTestData.TestSet);

        // Now that we have the predictions, calculate the metrics of those predictions and output the results.
        CalibratedBinaryClassificationMetrics? metrics = mlContext.BinaryClassification.Evaluate(predictions);
        PrintBinaryClassificationMetrics(metrics);

        MLSettings? mlSettings = configuration.GetSection("ML").Get<MLSettings>();
        mlContext.Model.Save(model, dataView.Schema, mlSettings.ModelPath);
    }

    private static async Task<IEnumerable<ML.Location>> QueryDataAsync(OwntracksSettings owntracksSettings, CancellationToken cancellationToken = default)
    {
        using var messageHandler = new HttpClientHandler { Credentials = new NetworkCredential(owntracksSettings.HttpUserName, owntracksSettings.HttpPassword) };
        using var httpClient = new HttpClient(messageHandler)
        {
            BaseAddress = owntracksSettings.Uri
        };

        var apiClient = new OwntracksApiClient(httpClient);
        GetUsersResult? users = await apiClient.GetUsersAsync(cancellationToken).ConfigureAwait(false);
        var result = new List<ML.Location>();
        if (users == null)
        {
            return result;
        }

        foreach (string? user in users.Results)
        {
            GetDevicesResult? devices = await apiClient.GetDevicesAsync(user, cancellationToken).ConfigureAwait(false);
            if (devices == null)
            {
                continue;
            }

            foreach (string? device in devices.Results)
            {
                GetLocationsResult? locationsResult = await apiClient.GetLocationsAsync(user, device, DateTimeOffset.UtcNow.AddYears(-42), cancellationToken: cancellationToken).ConfigureAwait(false);
                if (locationsResult == null)
                {
                    continue;
                }

                IEnumerable<ML.Location>? convertedLocationResults = locationsResult.Data.Select(l => LocationConverter.Convert(user, device, l));
                result.AddRange(convertedLocationResults);
            }
        }

        return result;
    }

    private static void PrintBinaryClassificationMetrics(BinaryClassificationMetrics metrics)
    {
        Console.WriteLine($"************************************************************");
        Console.WriteLine($"*       Metrics for the binary classification model      ");
        Console.WriteLine($"*-----------------------------------------------------------");
        Console.WriteLine($"*       Accuracy: {metrics.Accuracy:P2}");
        Console.WriteLine($"*       Area Under Curve:      {metrics.AreaUnderRocCurve:P2}");
        Console.WriteLine($"*       Area under Precision recall Curve:  {metrics.AreaUnderPrecisionRecallCurve:P2}");
        Console.WriteLine($"*       F1Score:  {metrics.F1Score:P2}");
        Console.WriteLine($"*       PositivePrecision:  {metrics.PositivePrecision:#.##}");
        Console.WriteLine($"*       PositiveRecall:  {metrics.PositiveRecall:#.##}");
        Console.WriteLine($"*       NegativePrecision:  {metrics.NegativePrecision:#.##}");
        Console.WriteLine($"*       NegativeRecall:  {metrics.NegativeRecall:P2}");
        Console.WriteLine($"************************************************************");
    }
}
