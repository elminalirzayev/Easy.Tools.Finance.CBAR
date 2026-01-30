using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http;
using System.Xml.Serialization;

namespace Easy.Tools.Finance.CBAR
{
    /// <summary>
    /// High-performance implementation of the CBAR Client.
    /// </summary>
    public class CbarClient : ICbarClient
    {
        private readonly HttpClient _httpClient;
        private readonly CbarOptions _options;

        // PERF: XmlSerializer is made static to prevent memory leaks (cached instance).
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(CbarResponse));
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CbarClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client instance.</param>
        /// <param name="options">Configuration options.</param>

        public CbarClient(HttpClient httpClient, IOptions<CbarOptions> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<List<CbarCurrency>> GetTodayRatesAsync(CancellationToken cancellationToken = default)
        {
            return await GetRatesByDateAsync(DateTime.Now, cancellationToken);
        }

        public async Task<List<CbarCurrency>> GetRatesByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            // CULTURE SAFE: Date format is independent of the server's local culture.
            string dateStr = date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            string url = $"{_options.BaseUrl.TrimEnd('/')}/{dateStr}.xml";

            for (int i = 0; i < _options.RetryCount; i++)
            {
                try
                {
                    // FIX: GetStreamAsync(url, token) is not available in older .NET versions.
                    // We use GetAsync with ResponseHeadersRead for better performance and compatibility.

                    // 1. Send request (Return as soon as headers are received, don't wait for the whole body)
                    using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                    // 2. Ensure HTTP 200 OK
                    response.EnsureSuccessStatusCode();

                    // 3. Get the stream
#if NET5_0_OR_GREATER
                    using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#else
                    // Legacy versions do not support cancellation token in ReadAsStreamAsync
                    using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif

                    // 4. Deserialize
                    if (_serializer.Deserialize(responseStream) is CbarResponse xmlResponse && xmlResponse.ValTypes != null)
                    {
                        var allCurrencies = new List<CbarCurrency>();

                        foreach (var valType in xmlResponse.ValTypes)
                        {
                            if (valType.Currencies == null) continue;

                            foreach (var currency in valType.Currencies)
                            {
                                // Flattening: Assign the Parent Type (e.g., "Foreign Currency") to the Child
                                currency.CurrencyType = valType.Type;
                                allCurrencies.Add(currency);
                            }
                        }

                        return allCurrencies;
                    }
                }
                catch (HttpRequestException)
                {
                    // If it is the last attempt, rethrow the exception.
                    if (i == _options.RetryCount - 1) throw;

                    // Exponential Backoff or fixed delay
                    await Task.Delay(TimeSpan.FromSeconds(_options.RetryDelaySeconds), cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    throw; // If cancelled, exit immediately. Do not retry.
                }
                catch (Exception)
                {
                    throw; // Rethrow other exceptions (e.g., XML parsing errors).
                }
            }

            return new List<CbarCurrency>();
        }
    }
}