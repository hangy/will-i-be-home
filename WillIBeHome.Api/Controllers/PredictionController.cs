namespace WillIBeHome.Api.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.ML;
    using WillIBeHome.ML;
    using WillIBeHome.Owntracks;
    using WillIBeHome.Shared;

    [ApiController]
    [Route("[controller]")]
    public class PredictionController : ControllerBase
    {
        private readonly IOwntracksApiClient owntracksApiClient;

        private readonly PredictionEnginePool<Transition, WillBeHomePrediction> predictionEnginePool;

        public PredictionController(IOwntracksApiClient owntracksApiClient, PredictionEnginePool<Transition, WillBeHomePrediction> predictionEnginePool)
        {
            this.owntracksApiClient = owntracksApiClient ?? throw new ArgumentNullException(nameof(owntracksApiClient));
            this.predictionEnginePool = predictionEnginePool ?? throw new ArgumentNullException(nameof(predictionEnginePool));
        }

        [HttpGet]
        public async Task<bool> Get(string user, string device, CancellationToken cancellationToken = default)
        {
            var locations = await this.owntracksApiClient.GetLocationsAsync(user, device, DateTimeOffset.UtcNow.AddDays(-1), cancellationToken: cancellationToken).ConfigureAwait(false);
            if (locations== null)
            {
                return false;
            }

            var transitions = LocationsToTransitionsConverter.Convert(locations.Data.Select(l => LocationConverter.Convert(user, device, l)));
            var lastTransition = transitions.LastOrDefault();
            if (lastTransition == null)
            {
                return false;
            }

            var predictionResult = this.predictionEnginePool.Predict(lastTransition);
            return predictionResult.Label;
        }
    }
}
