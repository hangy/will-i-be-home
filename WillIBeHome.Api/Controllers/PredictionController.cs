using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using WillIBeHome.ML;
using WillIBeHome.Owntracks;
using WillIBeHome.Shared;

namespace WillIBeHome.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictionController : ControllerBase
    {
        private readonly IOwntracksApiClient _owntracksApiClient;

        private readonly PredictionEnginePool<Transition, WillBeHomePrediction> _predictionEnginePool;

        public PredictionController(IOwntracksApiClient owntracksApiClient, PredictionEnginePool<Transition, WillBeHomePrediction> predictionEnginePool)
        {
            _owntracksApiClient = owntracksApiClient ?? throw new ArgumentNullException(nameof(owntracksApiClient));
            _predictionEnginePool = predictionEnginePool ?? throw new ArgumentNullException(nameof(predictionEnginePool));
        }

        [HttpGet]
        public async Task<bool> Get(string user, string device, CancellationToken cancellationToken = default)
        {
            GetLocationsResult? locations = await _owntracksApiClient.GetLocationsAsync(user, device, DateTimeOffset.UtcNow.AddDays(-1), cancellationToken: cancellationToken).ConfigureAwait(false);
            if (locations== null)
            {
                return false;
            }

            System.Collections.Generic.IEnumerable<Transition>? transitions = LocationsToTransitionsConverter.Convert(locations.Data.Select(l => LocationConverter.Convert(user, device, l)));
            Transition? lastTransition = transitions.LastOrDefault();
            if (lastTransition == null)
            {
                return false;
            }

            WillBeHomePrediction? predictionResult = _predictionEnginePool.Predict(lastTransition);
            return predictionResult.Label;
        }
    }
}
