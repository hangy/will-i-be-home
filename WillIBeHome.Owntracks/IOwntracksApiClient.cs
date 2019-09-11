using System;
using System.Threading;
using System.Threading.Tasks;

namespace WillIBeHome.Owntracks
{
    public interface IOwntracksApiClient
    {
        Task<GetDevicesResult> GetDevicesAsync(string user, CancellationToken cancellationToken = default);
        Task<GetLocationsResult> GetLocationsAsync(string user, string device, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default);
        Task<GetUsersResult> GetUsersAsync(CancellationToken cancellationToken = default);
    }
}