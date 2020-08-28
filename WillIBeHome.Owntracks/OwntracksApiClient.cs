﻿namespace WillIBeHome.Owntracks
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    public class OwntracksApiClient : IOwntracksApiClient
    {
        private readonly HttpClient httpClient;

        public OwntracksApiClient(HttpClient httpClient) => this.httpClient = httpClient;

        public async Task<GetUsersResult> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            const string uri = "api/0/list";
            using var response = await this.httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync<GetUsersResult>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<GetDevicesResult> GetDevicesAsync(string user, CancellationToken cancellationToken = default)
        {
            var uri = $"api/0/list?user={HttpUtility.UrlEncode(user)}";
            using var response = await this.httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync<GetDevicesResult>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<GetLocationsResult> GetLocationsAsync(string user, string device, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default)
        {
            var uriBuilder = new StringBuilder($"api/0/locations?user={HttpUtility.UrlEncode(user)}&device={HttpUtility.UrlEncode(device)}");
            if (from.HasValue)
            {
                uriBuilder = uriBuilder.Append("&from=").Append(HttpUtility.UrlEncode(from.Value.ToString("o", CultureInfo.InvariantCulture)));
            }

            if (to.HasValue)
            {
                uriBuilder = uriBuilder.Append("&to=").Append(HttpUtility.UrlEncode(to.Value.ToString("o", CultureInfo.InvariantCulture)));
            }

            var uri = uriBuilder.ToString();
            using var response = await this.httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync<GetLocationsResult>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
